using System.ComponentModel.DataAnnotations;

namespace Manage_Target.Models
{
    public class Item
    {
        public long Id { get; set; }
        
        public string? Name { get; set; }
        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        [Range(0f, 6000000f)]       // Unit USD
        public float OpenCost { get; set; }

        public float RemainingCost { get; set; }
    }

    public class Task
    {
        public long Id { get; set; }
        public long IdItem { get; set; }

        public string? Name { get; set; }
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        [Range(0f, 6000000f)]       // Unit USD
        public float ActualCost { get; set; }
    }
}
