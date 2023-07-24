using ReportAPI.Models;

namespace Manage_Target.Context
{
    public interface IReportRepo
    {
        bool SaveChanges();

        // Report
        IEnumerable<Report> GetAllReports();
        void CreateReport(Report report);
        bool ModifyReportState(Report report);
        bool RemoveReportItem(long itemId);
        bool RemoveReportTask(long taskId);
        bool ReportExits(long itemId, long? taskId = null);

    }
}
