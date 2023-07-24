using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportAPI.Models;
using UserAPI.Context;

namespace Manage_Target.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly UserContext _context;

        public ReportController(UserContext ctx)
        {
            _context = ctx;
        }
        [HttpGet]
        public async Task<IEnumerable<Report>> GetAll()
        {
            var items = await _context.Reports.ToListAsync();
            if (items == null || !items.Any())
            {
                return new List<Report>();
            }
            return items.OrderBy(i => i.Start);
        }
        [HttpGet("item/")]
        public async Task<IEnumerable<Report>> GetAllItem()
        {
            var items = await _context.Reports.ToListAsync();
            if (items == null || !items.Any())
            {
                return new List<Report>();
            }
            return items.Where(x => x.IdTask == null).OrderBy(i => i.Start);
        }
        [HttpGet("item/{id}")]
        public async Task<IEnumerable<Report>> GetAllTaskBy(long id)
        {
            var items = await _context.Reports.ToListAsync();
            if (items == null || !items.Any())
            {
                return new List<Report>();
            }
            return items.Where(x => x.IdItem == id).OrderBy(i => i.Start);
        }
    }
}
