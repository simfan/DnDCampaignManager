using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class Entry
    {
        public int EntryId { get; set; }
        public int JournalId { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool isPrivate { get; set; }
        public List<Tag>? Tags { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        public Journal Journal { get; set; }
    }
}
