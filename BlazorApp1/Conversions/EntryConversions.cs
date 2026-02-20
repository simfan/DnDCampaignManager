using BlazorApp1.DTOs;
using BlazorApp1.Models;

namespace BlazorApp1.Conversions
{
    public static class EntryConversions
    {
        public static Entry DtoToEntry(EntryDto dto)
        {
            return new Entry()
            {
                EntryId = dto.EntryId,
                JournalId = dto.JournalId,
                AuthorId = dto.AuthorId,
                Title = dto.Title,
                Content = dto.Content,
                isPrivate = dto.IsPrivate,
                CreatedAt = dto.CreatedAt,
                LastUpdatedAt = dto.LastUpdatedAt
            };
        }

        public static Entry DtoDetailToEntry(EntryDetailDto dto)
        {
            var entry = new Entry()
            {
                EntryId = dto.EntryId,
                JournalId = dto.JournalId,
                AuthorId = dto.AuthorId,
                Title = dto.Title,
                Content = dto.Content,
                isPrivate = dto.IsPrivate,
                CreatedAt = dto.CreatedAt,
                LastUpdatedAt = dto.LastUpdatedAt,
                Tags = new()
            };

            foreach(var dtoTag in entry.Tags)
            {
                var tag = new EntryTag()
                {
                    TagId = dtoTag.TagId,
                    EntryId = dtoTag.EntryId
                };
                entry.Tags.Add(tag);
            }
            return entry;
        }
        public static EntryDto EntryToDto(Entry entry)
        {
            return new EntryDto()
            {
                EntryId = entry.EntryId,
                JournalId = entry.JournalId,
                AuthorId = entry.AuthorId,
                Title = entry.Title,
                Content = entry.Content,
                IsPrivate = entry.isPrivate,
                CreatedAt = entry.CreatedAt,
                LastUpdatedAt = entry.LastUpdatedAt
            };
        }

        public static Entry CreateToEntry(CreateEntryDto dto)
        {
            DateTime currentDate = DateTime.Now;
            return new Entry()
            {
                EntryId = 0,
                JournalId = dto.JournalId,
                AuthorId = dto.AuthorId,
                Title = dto.Title,
                Content = dto.Content,
                isPrivate = dto.IsPrivate,
                CreatedAt = currentDate,
                LastUpdatedAt = currentDate
            };
        }

        public static Entry UpdateToEntry(UpdateEntryDto dto, DateTime createdDate)
        {
            DateTime currentDate = DateTime.Now;
            return new Entry()
            {
                EntryId = dto.EntryId,
                JournalId = dto.JournalId,
                AuthorId = dto.AuthorId,
                Title = dto.Title,
                Content = dto.Content,
                isPrivate = dto.IsPrivate,
                CreatedAt = createdDate,
                LastUpdatedAt = currentDate
            };
        }

        public static EntryDetailDto EntryToDetailDto(Entry entry)
        {
            List<TagDto>? tags = new();
            foreach (var tag in entry.Tags)
            {
                var tagDto = new TagDto()
                {
                    TagId = tag.TagId,
                    Name = tag.Tag.Name
                };
                tags.Add(tagDto);
            }

            return new EntryDetailDto()
            {
                EntryId = entry.EntryId,
                JournalId = entry.JournalId,
                AuthorId = entry.AuthorId,
                Title = entry.Title,
                Content = entry.Content,
                IsPrivate = entry.isPrivate,
                CreatedAt = entry.CreatedAt,
                LastUpdatedAt = entry.LastUpdatedAt,
                Tags = tags
            };
        }
    }
}
