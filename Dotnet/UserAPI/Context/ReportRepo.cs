using Microsoft.EntityFrameworkCore;
using ReportAPI.Models;
using UserAPI.Context;

namespace Manage_Target.Context
{
    public class ReportRepo : IReportRepo
    {
        private readonly UserContext _context;
        public ReportRepo(UserContext context)
        {
            _context = context;
        }

        public void CreateReport(Report report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }
            _context.Reports.Add(report);
        }
        public bool ModifyReportState(Report report)
        {
            if (report == null) return false;
            var dbReport = _context.Reports.FirstOrDefault(x => x.IdItem == report.IdItem && x.IdTask== report.IdTask);
            if (dbReport == null) return false;
            dbReport.Name= report.Name;
            dbReport.Start= report.Start;
            dbReport.End = report.End;
            dbReport.OpenCost = report.OpenCost;
            dbReport.ActualCost = report.ActualCost;
            _context.Reports.Entry(dbReport).State = EntityState.Modified;
            return true;
        }
        public bool RemoveReportItem(long itemId)
        {
            if (itemId <= 0) return false;
            var report = _context.Reports.FirstOrDefault(x => x.IdItem == itemId);
            if(report != null)
            {
                _context.Reports.Remove(report);
                var tasks = _context.Reports.Where(x => x.IdTask != null && x.IdTask > 0 && x.IdItem == itemId);
                _context.Reports.RemoveRange(tasks);
                return true;
            }
            return false;
        }
        public bool RemoveReportTask(long taskId)
        {
            if (taskId <= 0) return false;
            var report = _context.Reports.FirstOrDefault(x => x.IdTask == taskId);
            if(report != null)
            {
                _context.Reports.Remove(report);
                return true;
            }
            return false;
        }

        public IEnumerable<Report> GetAllReports()
        {
            return _context.Reports.ToList();
        }

        public bool ReportExits(long itemId, long? taskId = null)
        {
            return _context.Reports.Any(p => p.IdItem == itemId && taskId == p.IdTask);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
