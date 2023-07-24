using System.Diagnostics.CodeAnalysis;

namespace ReportAPI.Models
{
    public class Report
    {
        public long Id { get; set; }

        [AllowNull]
        public long? IdItem { get; set; }

        [AllowNull]
        public long? IdTask { get; set; }
        public string Name { get; set; }

        [AllowNull]
        public DateTime? Start { get; set; }

        [AllowNull]
        public DateTime? End { get; set; }

        [AllowNull]
        public float? OpenCost { get; set; }

        [AllowNull]
        public float? ActualCost { get; set; }
    }
}
