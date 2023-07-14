using Manage_Target.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_Target.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ManageContext _context;
        public TaskController(ManageContext ctx)
        {
            _context= ctx;
        }
        #region Get
        [HttpGet]
        public async Task<IEnumerable<Models.Task>> GetAllTask()
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
        public async Task<ActionResult<Models.Task>> GetTask(long id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }
        [HttpGet("item/{id}")]
        public async Task<IEnumerable<Models.Task>> GetTasksByItemId(long id)
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
            if(items != null && !items.Any(i => i.Id == task.IdItem))
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
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        private bool TaskExists(long id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(long id, Models.Task task)
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
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
