using System.ComponentModel;

namespace Manage_Target.Models
{
    public class Settings
    {
        public long Id { get; set; }
        public long IdItem { get; set; }

        [DefaultValue("16:30-0")]
        public string ReportTime { get; set; }

        [DefaultValue("Light")]
        public string Theme { get; set; }

        [DefaultValue("en")]
        public string Language { get; set; }

        [DefaultValue("$")]
        public string PriceUnit { get; set; }
    }
}
