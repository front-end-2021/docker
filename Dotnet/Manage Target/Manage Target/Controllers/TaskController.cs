using Manage_Target.Context;
using Manage_Target.DataServices.AsyncBusClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_Target.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ManageContext _context;
        private readonly IMessageBusClient _messageBusClient;
        public TaskController(ManageContext ctx, IMessageBusClient bClient)
        {
            _context= ctx;
            _messageBusClient = bClient;
        }
        #region Get
        [HttpGet]
        public async Task<IEnumerable<Models.Task>> GetAll()
        {
            var tasks = await _context.Tasks.ToListAsync();
            if (tasks == null || !tasks.Any())
            {
                return Enumerable.Range(1, 5).Select(index => new Models.Task
                {
                    Id = index,
                    Name = $"Test TAsk {index}",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(index)
                }).ToArray();
            }
            return tasks.OrderBy(t => t.IdItem).ThenBy(t => t.Start);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Task>> Get(long id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }
        [HttpGet("item/{id}")]
        public async Task<IEnumerable<Models.Task>> GetByItemId(long id)
        {
            var tasks = await _context.Tasks.ToListAsync();
            if (tasks == null || !tasks.Any())
            {
                return new List<Models.Task>();
            }
            return tasks.Where(t => t.IdItem == id).OrderBy(t => t.IdItem).ThenBy(t => t.Start);
        }
        #endregion Get
        [HttpPost]
        public async Task<ActionResult<Models.Task>> Create(Models.Task task)
        {
            if(task.IdItem < 1)
            {
                return NotFound("Could not create Task without IdItem");
            }
            var items = await _context.Items.ToListAsync();
            if (items == null)
            {
                return NotFound("Has not any Items");
            }
            var item = items.FirstOrDefault(x => x.Id == task.IdItem);
            if (item == null)
            {
                return NotFound($"Could not find Item by IdItem = {task.IdItem}");
            }
            try
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            #region Send Async message
            PublishAsyncMessage(task, "Add_Report_Task");
            #endregion
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }
        private void PublishAsyncMessage(Models.Task task, string strEvent)
        {
            if (string.IsNullOrEmpty(task.Name)) return;
            try
            {
                var publicTask = new TaskPublishDto()
                {
                    Id = task.Id,
                    IdItem = task.IdItem,
                    Name = task.Name,
                    Start = task.Start,
                    End = task.End,
                    ActualCost = task.ActualCost,
                    Event = strEvent
                };
                _messageBusClient.PublishEntry(publicTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send asynchronously: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }
            if (task.IdItem < 1)
            {
                return NotFound("Could not update Task without IdItem");
            }
            var items = await _context.Items.ToListAsync();
            if (items != null && !items.Any(i => i.Id == task.IdItem))
            {
                return NotFound($"Could not find Item by IdItem = {task.IdItem}");
            }

            _context.Tasks.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                PublishAsyncMessage(task, "Update_Report_Task");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tasks.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Accepted();
        }
        private void DeleteAsyncMessage(long id)
        {
            #region Send Async message
            try
            {
                var entryDto = new EntryDeleteDto()
                {
                    Id = id,
                    Event = "Delete_Report_Task"
                };
                _messageBusClient.PublishEntry(entryDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send asynchronously: {ex.Message}");
            }
            #endregion
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            DeleteAsyncMessage(id);
            return Ok();
        }
    }
}
