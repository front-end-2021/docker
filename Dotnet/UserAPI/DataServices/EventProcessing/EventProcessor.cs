using Manage_Target.Context;
using ReportAPI.Models;
using System.Text.Json;

namespace Manage_Target.DataServices.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scope)
        {
            _scopeFactory = scope;
        }
        public void ProcessEvent(string message)
        {
            var eventType = JsonSerializer.Deserialize<EventDto>(message);
            if (eventType == null) return;
            switch(eventType.Event)
            {
                case "Add_Report_Item":
                    AddItem(message);
                    break;
                case "Update_Report_Item":
                    UpdateItem(message);
                    break;
                case "Delete_Report_Item":
                    DeleteItem(message);
                    break;
                case "Add_Report_Task":
                    AddTask(message);
                    break;
                case "Update_Report_Task":
                    UpdateTask(message);
                    break;
                case "Delete_Report_Task":
                    DeleteTask(message);
                    break;
                default: break;
            }
        }
        private void AddTask(string message)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IReportRepo>();
                var task = JsonSerializer.Deserialize<TaskPublishDto>(message);
                if (task == null) return;
                try
                {
                    var report = new Report() {
                        IdItem = task.IdItem,
                        IdTask = task.Id,
                        Name = task.Name,
                        Start = task.Start,
                        End = task.End,
                        ActualCost = task.ActualCost
                    };
                    if (repo.ReportExits(task.IdItem, task.Id))
                    {
                        Console.WriteLine("--> Task Report already exisits...");
                    } else
                    {
                        repo.CreateReport(report);
                        repo.SaveChanges();
                    }
                } 
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not add Task Report to DB {ex.Message}");
                }
            }
        }
        private void UpdateTask(string message)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IReportRepo>();
                var item = JsonSerializer.Deserialize<TaskPublishDto>(message);
                if (item == null) return;
                var report = new Report()
                {
                    IdTask = item.Id,
                    IdItem = item.IdItem,
                    Name = item.Name,
                    Start = item.Start,
                    End = item.End,
                    ActualCost = item.ActualCost
                };
                if (repo.ModifyReportState(report))
                    repo.SaveChanges();
            }
        }
        private void DeleteTask(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IReportRepo>();
                var item = JsonSerializer.Deserialize<EntryDeleteDto>(message);
                if (item == null) return;
                if (repo.RemoveReportTask(item.Id))
                    repo.SaveChanges();
            }
        }

        private void DeleteItem(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IReportRepo>();
                var item = JsonSerializer.Deserialize<EntryDeleteDto>(message);
                if (item == null) return;
                if (repo.RemoveReportItem(item.Id))
                    repo.SaveChanges();
            }
        }
        private void UpdateItem(string message)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IReportRepo>();
                var item = JsonSerializer.Deserialize<ItemPublishDto>(message);
                if (item == null) return;
                var report = new Report()
                {
                    IdItem = item.Id,
                    Name = item.Name,
                    Start = item.Start,
                    End = item.End,
                    OpenCost = item.OpenCost
                };
                if (repo.ModifyReportState(report))
                    repo.SaveChanges();
            }
        }
        private void AddItem(string message)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IReportRepo>();
                var item = JsonSerializer.Deserialize<ItemPublishDto>(message);
                if (item == null) return;
                try
                {
                    var report = new Report() {
                        IdItem = item.Id,
                        Name = item.Name,
                        Start = item.Start,
                        End = item.End,
                        OpenCost = item.OpenCost
                    };
                    if (repo.ReportExits(item.Id))
                    {
                        Console.WriteLine("--> Item Report already exisits...");
                    } else
                    {
                        repo.CreateReport(report);
                        repo.SaveChanges();
                    }
                } 
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not add Item Report to DB {ex.Message}");
                }
            }
        }
    }
    public class EventDto
    {
        public string Event { get; set; }
    }
    public class ItemPublishDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public float OpenCost { get; set; }
    }
    public class TaskPublishDto
    {
        public long Id { get; set; }
        public long IdItem { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public float ActualCost { get; set; }
    }
    public class EntryDeleteDto
    {
        public long Id { get; set; }
    }
}
