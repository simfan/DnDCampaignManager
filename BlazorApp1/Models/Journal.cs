using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class Journal
    {
        public int JournalId { get; set; }
        public string Name { get; set; }
        public int JournalTypeId { get; set; }
        public string OwnerId { get; set;  } = string.Empty;
        public List<Entry>? Entries { get; set; }
        public List<Tag>? Tags { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
