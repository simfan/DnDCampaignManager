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

        public EntryDetailDto ToDetailDto()
        {
            return new EntryDetailDto
            {
                EntryId = EntryId,
                JournalId = JournalId,
                AuthorId = AuthorId,
                Title = Title,
                Content = Content,
                IsPrivate = IsPrivate,
                CreatedAt = CreatedAt,
                LastUpdatedAt = LastUpdatedAt,
                Tags = new()
            };
        }
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
            public EntryDto ToDto()
            {
                return new EntryDto
                {
                    EntryId = EntryId,
                    JournalId = JournalId,
                    AuthorId = AuthorId,
                    Title = Title,
                    Content = Content,
                    IsPrivate = IsPrivate,
                    CreatedAt = CreatedAt,
                    LastUpdatedAt = LastUpdatedAt
                };
            }
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
