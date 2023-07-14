
namespace Manage_Target.Models
{
    public class Task
    {
        public long Id { get; set; }
        public long IdItem { get; set; }

        public string? Name { get; set; }
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public float ActualCost { get; set; }
    }
}
