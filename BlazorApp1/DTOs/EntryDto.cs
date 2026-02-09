namespace BlazorApp1.DTOs
{
    public class EntryDto
    {
        public int EntryId { get; set; }
        public int JournalId { get; set; }
        public string AuthorId {  get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime? CreatedAt {  get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }

    public class EntryDetailDto
    {
        public int EntryId { get; set; }
        public int JournalId { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public List<TagDto>? Tags { get; set; }
    }

    public class CreateEntryDto
    {
        public int JournalId { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPrivate { get; set; }
    }

    public class UpdateEntryDto 
    {
        public int EntryId { get; set; }
        public int JournalId { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPrivate { get; set; }
    }

}
