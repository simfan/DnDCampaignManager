using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class EntryTag
    {
        public int EntryId { get; set; }
        public int TagId { get; set; }

        public Entry Entry { get; set; }
        public Tag Tag { get; set; }
    }
}
