using Manage_Target.Context;
using Manage_Target.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_Target.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly ManageContext _context;

        public ItemController(ManageContext ctx, ILogger<ItemController> logger)
        {
            _context = ctx;
            _logger = logger;
        }
        #region Get
        [HttpGet]
        public async Task<IEnumerable<Item>> GetAllItem()
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
        public async Task<ActionResult<Item>> GetItem(long id)
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
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }
        private bool ItemExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(long id, [Bind("Id,Name,Start,End,OpenCost")]Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Items.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
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
        public async Task<IActionResult> DeleteItem(long id)
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

            return Ok();
        }
    }
}