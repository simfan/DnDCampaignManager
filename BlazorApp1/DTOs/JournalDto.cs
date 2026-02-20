namespace BlazorApp1.DTOs
{
    public class JournalDto
    {
        public int JournalId { get; set; }
        public string Name { get; set; }
        public int JournalTypeId {  get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedAt {  get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public JournalDetailDto ToDetalDto()
        {
            return new JournalDetailDto
            {
                JournalId = JournalId,
                Name = Name,
                JournalTypeId = JournalTypeId,
                OwnerId = OwnerId,
                CreatedAt = CreatedAt,
                LastUpdatedAt = LastUpdatedAt,
                Entries = new(),
                Tags = new()
            };
        }
    }

    public class JournalDetailDto
    {
        public int JournalId { get; set; }
        public string Name { get; set; }
        public int JournalTypeId { get; set; }
        public string OwnerId { get; set; }
        public List<EntryDetailDto>? Entries { get; set; }
        public List<TagDto>? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public JournalDto ToDto()
        {
            return new JournalDto
            {
                JournalId = JournalId,
                Name = Name,
                JournalTypeId = JournalTypeId,
                OwnerId = OwnerId,
                CreatedAt = CreatedAt,
                LastUpdatedAt = LastUpdatedAt
            };
        }
    }

    public class CreateJournalDto 
    {
        public string Name { get; set; }
        public int JournalTypeId { get; set; }
        public string OwnerId { get; set; }
    }

    public class UpdateJournalDto
    {
        public int JournalId { get; set; }
        public string Name { get; set; }
        public int JournalTypeId { get; set; }
        public string OwnerId { get; set; }
        //public DateTime CreatedAt { get; set; }
    }
}
