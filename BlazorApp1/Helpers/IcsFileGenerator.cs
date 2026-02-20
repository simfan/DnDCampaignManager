using BlazorApp1.DTOs;
using System.Text;

namespace BlazorApp1.Helpers
{
    public static class IcsFileGenerator
    {
        public static string GenerateIcsFile(PlayerEventDto playerEvent)
        {
            var sb = new StringBuilder();

            // ICS header
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//D&D Campaign Manager//Event Export//EN");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");

            // Event
            sb.AppendLine("BEGIN:VEVENT");

            // Unique ID
            sb.AppendLine($"UID:{playerEvent.PlayerEventId}@dndcampaign.local");

            // Timestamp
            sb.AppendLine($"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");

            // Start date/time
            if (playerEvent.StartTime.HasValue)
            {
                var startDateTime = playerEvent.EventDate.Date + playerEvent.StartTime.Value;
                sb.AppendLine($"DTSTART:{FormatDateTime(startDateTime)}");
            }
            else
            {
                // All-day event
                sb.AppendLine($"DTSTART;VALUE=DATE:{playerEvent.EventDate:yyyyMMdd}");
            }

            // End date/time
            if (playerEvent.EndTime.HasValue)
            {
                var endDateTime = playerEvent.EventDate.Date + playerEvent.EndTime.Value;
                sb.AppendLine($"DTEND:{FormatDateTime(endDateTime)}");
            }
            else if (playerEvent.StartTime.HasValue)
            {
                // If has start time but no end time, default to 2 hours
                var endDateTime = playerEvent.EventDate.Date + playerEvent.StartTime.Value + TimeSpan.FromHours(2);
                sb.AppendLine($"DTEND:{FormatDateTime(endDateTime)}");
            }
            else
            {
                // All-day event ends next day
                sb.AppendLine($"DTEND;VALUE=DATE:{playerEvent.EventDate.AddDays(1):yyyyMMdd}");
            }

            // Title/Summary
            sb.AppendLine($"SUMMARY:{EscapeText(playerEvent.Title)}");

            // Description
            var description = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(playerEvent.Description))
            {
                description.Append(playerEvent.Description);
            }
            if (!string.IsNullOrWhiteSpace(playerEvent.CampaignName))
            {
                if (description.Length > 0)
                    description.Append("\\n\\n");
                description.Append($"Campaign: {playerEvent.CampaignName}");
            }
            if (description.Length > 0)
            {
                sb.AppendLine($"DESCRIPTION:{EscapeText(description.ToString())}");
            }

            // Location (optional - could add campaign name here too)
            if (!string.IsNullOrWhiteSpace(playerEvent.CampaignName))
            {
                sb.AppendLine($"LOCATION:{EscapeText(playerEvent.CampaignName)}");
            }

            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }

        public static string GenerateIcsFile(IEnumerable<PlayerEventDto> playerEvents)
        {
            var sb = new StringBuilder();

            // ICS header
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//D&D Campaign Manager//Event Export//EN");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");

            foreach (var playerEvent in playerEvents)
            {
                // Event
                sb.AppendLine("BEGIN:VEVENT");

                // Unique ID
                sb.AppendLine($"UID:{playerEvent.PlayerEventId}@dndcampaign.local");

                // Timestamp
                sb.AppendLine($"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");

                // Start date/time
                if (playerEvent.StartTime.HasValue)
                {
                    var startDateTime = playerEvent.EventDate.Date + playerEvent.StartTime.Value;
                    sb.AppendLine($"DTSTART:{FormatDateTime(startDateTime)}");
                }
                else
                {
                    // All-day event
                    sb.AppendLine($"DTSTART;VALUE=DATE:{playerEvent.EventDate:yyyyMMdd}");
                }

                // End date/time
                if (playerEvent.EndTime.HasValue)
                {
                    var endDateTime = playerEvent.EventDate.Date + playerEvent.EndTime.Value;
                    sb.AppendLine($"DTEND:{FormatDateTime(endDateTime)}");
                }
                else if (playerEvent.StartTime.HasValue)
                {
                    // If has start time but no end time, default to 2 hours
                    var endDateTime = playerEvent.EventDate.Date + playerEvent.StartTime.Value + TimeSpan.FromHours(2);
                    sb.AppendLine($"DTEND:{FormatDateTime(endDateTime)}");
                }
                else
                {
                    // All-day event ends next day
                    sb.AppendLine($"DTEND;VALUE=DATE:{playerEvent.EventDate.AddDays(1):yyyyMMdd}");
                }

                // Title/Summary
                sb.AppendLine($"SUMMARY:{EscapeText(playerEvent.Title)}");

                // Description
                var description = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(playerEvent.Description))
                {
                    description.Append(playerEvent.Description);
                }
                if (!string.IsNullOrWhiteSpace(playerEvent.CampaignName))
                {
                    if (description.Length > 0)
                        description.Append("\\n\\n");
                    description.Append($"Campaign: {playerEvent.CampaignName}");
                }
                if (description.Length > 0)
                {
                    sb.AppendLine($"DESCRIPTION:{EscapeText(description.ToString())}");
                }

                // Location (optional - could add campaign name here too)
                if (!string.IsNullOrWhiteSpace(playerEvent.CampaignName))
                {
                    sb.AppendLine($"LOCATION:{EscapeText(playerEvent.CampaignName)}");
                }

                sb.AppendLine("END:VEVENT");
            }

            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }

        private static string FormatDateTime(DateTime dateTime)
        {
            // Convert to UTC and format as ISO 8601
            var utcDateTime = dateTime.ToUniversalTime();
            return utcDateTime.ToString("yyyyMMddTHHmmssZ");
        }

        private static string EscapeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Escape special characters for ICS format
            return text
                .Replace("\\", "\\\\")
                .Replace(",", "\\,")
                .Replace(";", "\\;")
                .Replace("\n", "\\n")
                .Replace("\r", "");
        }
    }
}