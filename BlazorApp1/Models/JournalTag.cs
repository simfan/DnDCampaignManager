using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class JournalTag
    {
        public int JournalId { get; set; }
        public int TagId { get; set; }

        public Journal Journal { get; set; }
        public Tag Tag { get; set; }
    }
}
