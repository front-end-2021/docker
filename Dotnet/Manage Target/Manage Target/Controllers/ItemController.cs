using Manage_Target.Context;
using Manage_Target.DataServices.AsyncBusClient;
using Manage_Target.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_Target.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ManageContext _context;
        private readonly IMessageBusClient _messageBusClient;

        public ItemController(ManageContext ctx, IMessageBusClient bClient)
        {
            _context = ctx;
            _messageBusClient = bClient;
        }
        #region Get
        [HttpGet]
        public async Task<IEnumerable<Item>> GetAll()
        {
            var items = await _context.Items.ToListAsync();
            if(items == null || !items.Any())
            {
                return Enumerable.Range(1, 5).Select(index => new Item
                {
                    Id = index,
                    Name = $"Test Item {index}",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(index)
                }).ToArray();
            }

            var tasks = await _context.Tasks.ToListAsync();
            if (tasks != null && tasks.Any())
            {
                Parallel.ForEach(items, (item) =>
                {
                    var itemTasks = tasks.Where(t => t.IdItem == item.Id);
                    var aCost = itemTasks.Sum(t => t.ActualCost);
                    item.RemainingCost = (float)Math.Round(item.OpenCost - aCost, 2);
                });
            }

            return items.OrderBy(i => i.Start);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> Get(long id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.ToListAsync();
            if (tasks != null && tasks.Any(t => t.IdItem == item.Id))
            {
                var itemTasks = tasks.Where(t => t.IdItem == item.Id);
                var aCost = itemTasks.Sum(t => t.ActualCost);
                item.RemainingCost = (float)Math.Round(item.OpenCost - aCost, 2);
            }

            return item;
        }
        #endregion Get

        [HttpPost]
        public async Task<ActionResult<Item>> Create([Bind("Id,Name,Start,End,OpenCost")]Item item)
        {
            try
            {
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex )
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            #region Send Async message
            if(item.OpenCost> 0)
            {
                PublishAsyncMessage(item, "Add_Report_Item");
            }
            #endregion
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }
        private void PublishAsyncMessage(Item item, string strEvent)
        {
            if (string.IsNullOrEmpty(item.Name)) return;
            try
            {
                var publicItem = new ItemPublishDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Start = item.Start,
                    End = item.End,
                    OpenCost = item.OpenCost,
                    Event = strEvent
                };
                _messageBusClient.PublishEntry(publicItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send asynchronously: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [Bind("Id,Name,Start,End,OpenCost")]Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Items.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                if(item.OpenCost <= 0)
                {
                    DeleteAsyncMessage(id);
                } else
                {
                    PublishAsyncMessage(item, "Update_Report_Item");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Items.Any(e => e.Id == id))
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
                    Event = "Delete_Report_Item"
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
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);

            var tasks = await _context.Tasks.ToListAsync();
            if(tasks != null && tasks.Any(t => t.IdItem == id))
            {
                var tasksRm = tasks.Where(t => t.IdItem == id);
                _context.Tasks.RemoveRange(tasksRm);
            }

            var settings = await _context.Settings.ToListAsync();
            if(settings != null && settings.Any(t => t.IdItem == id))
            {
                var settingsRm = settings.Where(t => t.IdItem == id);
                _context.Settings.RemoveRange(settingsRm);
            }

            await _context.SaveChangesAsync();

            DeleteAsyncMessage(id);
            return Ok();
        }
    }
}