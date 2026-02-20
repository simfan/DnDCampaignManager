using BlazorApp1.DTOs;
using BlazorApp1.Models;

namespace BlazorApp1.Conversions
{
    public static class JournalConversions
    {
        public static Journal DtoToJournal(JournalDto dto)
        {
            return new Journal()
            {
                JournalId = dto.JournalId,
                Name = dto.Name,
                JournalTypeId = dto.JournalTypeId,
                OwnerId = dto.OwnerId,
                CreatedAt = dto.CreatedAt,
                LastUpdatedAt = dto.LastUpdatedAt
            };
        }

        public static Journal DtoDetailToJournal(JournalDetailDto dto)
        {
            var journal = new Journal()
            {
                JournalId = dto.JournalId,
                Name = dto.Name,
                JournalTypeId = dto.JournalTypeId,
                OwnerId = dto.OwnerId,
                CreatedAt = dto.CreatedAt,
                LastUpdatedAt = dto.LastUpdatedAt,
                Entries = new(),
                Tags = new()
            };

            foreach (var dtoEntry in dto.Entries)
            {
                var entry = EntryConversions.DtoDetailToEntry(dtoEntry);
                journal.Entries.Add(entry);
            }

            foreach (var dtoTag in dto.Tags)
            {
                var journalTag = new JournalTag()
                {
                    TagId = dtoTag.TagId,
                    JournalId = dto.JournalId
                };
                                
                journal.Tags.Add(journalTag);
            }
            return journal;
        }

        public static Journal CreateToJournal(CreateJournalDto dto)
        {
            DateTime currentDate = DateTime.Now;
            return new Journal()
            {
                JournalId = 0,
                Name = dto.Name,
                JournalTypeId = dto.JournalTypeId,
                OwnerId = dto.OwnerId,
                CreatedAt = currentDate,
                LastUpdatedAt = currentDate

            };
        }

        public static JournalDto JournalToDto(Journal journal)
        {
            return new JournalDto()
            {
                JournalId = journal.JournalId,
                Name = journal.Name,
                JournalTypeId = journal.JournalTypeId,
                OwnerId = journal.OwnerId,
                CreatedAt = journal.CreatedAt,
                LastUpdatedAt = journal.LastUpdatedAt
            };
        }

        public static JournalDetailDto JournalToDetailDto(Journal journal)
        {
            var dto = new JournalDetailDto()
            {
                JournalId = journal.JournalId,
                Name = journal.Name,
                JournalTypeId = journal.JournalTypeId,
                OwnerId = journal.OwnerId,
                CreatedAt = journal.CreatedAt,
                LastUpdatedAt = journal.LastUpdatedAt,
                Entries = new(),
                Tags = new()
            };
            foreach(var entry in journal.Entries)
            {
                var entryDto = EntryConversions.EntryToDetailDto(entry);
                dto.Entries.Add(entryDto);
            }
            foreach(var tag in journal.Tags)
            {
                var tagDto = new TagDto()
                {
                    TagId = tag.TagId,
                    Name = tag.Tag.Name,
                };
                dto.Tags.Add(tagDto);
            }
            return dto;
                
        }
        public static Journal UpdateToJournal(UpdateJournalDto updatedJournal, DateTime createdAt)
        {
            return new Journal()
            {
                JournalId = updatedJournal.JournalId,
                Name = updatedJournal.Name,
                JournalTypeId = updatedJournal.JournalTypeId,
                OwnerId = updatedJournal.OwnerId,
                CreatedAt = createdAt,
                LastUpdatedAt = DateTime.Now
            };
        }
    }
}
