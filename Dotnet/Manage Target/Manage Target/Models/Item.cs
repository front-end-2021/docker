using System.Diagnostics.CodeAnalysis;

namespace Manage_Target.Models
{
    public class Item
    {
        public long Id { get; set; }
        
        public string? Name { get; set; }
        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public float OpenCost { get; set; }

        public float RemainingCost { get; set; }
    }
}
