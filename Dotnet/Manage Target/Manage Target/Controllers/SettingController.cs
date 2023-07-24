using Manage_Target.Context;
using Manage_Target.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_Target.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingController : ControllerBase
    {
        private readonly ManageContext _context;
        private readonly ILogger<ItemController> _logger;
        public SettingController(ManageContext ctx, ILogger<ItemController> logger)
        {
            _context = ctx;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IEnumerable<Settings>> GetAll()
        {
            var settings = await _context.Settings.ToListAsync();
            if (settings == null || !settings.Any())
            {
                return Enumerable.Range(1, 5).Select(index => new Settings
                {
                    Id = index,
                    ReportTime = "16:30-0"
                }).ToArray();
            }
            return settings;
        }
        [HttpGet("item/{id}")]
        public async Task<ActionResult<Settings>> GetByItemId(long id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var setting = await _context.Settings.FirstAsync(i => i.IdItem == id);
            if(setting == null) return NotFound();
            
            return setting;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Settings>> Get(long id)
        {
            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }
            return setting;
        }

        [HttpPost]
        public async Task<ActionResult<Settings>> Create(Settings setting)
        {
            if (setting.IdItem < 1)
            {
                return NotFound("Could not create Setting without IdItem");
            }
            var items = await _context.Items.ToListAsync();
            if (items != null && !items.Any(i => i.Id == setting.IdItem))
            {
                return NotFound($"Could not find Item by IdItem = {setting.IdItem}");
            }
            var dbSetting = await _context.Settings.FirstAsync(x => x.IdItem == setting.IdItem);
            if(dbSetting != null)
            {
                return Forbid($"Setting with IdItem = {setting.IdItem} is existed");
            }
            try
            {
                _context.Settings.Add(setting);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return CreatedAtAction(nameof(Get), new { id = setting.Id }, setting);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Settings entry)
        {
            if (id != entry.Id)
            {
                return BadRequest();
            }
            if (entry.IdItem < 1)
            {
                return NotFound("Could not update Setting without IdItem");
            }
            var items = await _context.Items.ToListAsync();
            if (items != null && !items.Any(i => i.Id == entry.IdItem))
            {
                return NotFound($"Could not find Item by IdItem = {entry.IdItem}");
            }

            _context.Settings.Entry(entry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var hasS = await _context.Settings.AnyAsync(e => e.Id == id);
                if (!hasS)
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
        public async Task<IActionResult> Delete(long id)
        {
            var entry = await _context.Settings.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            _context.Settings.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
