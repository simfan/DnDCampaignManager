using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BlazorApp1.DTOs;
using BlazorApp1.Models;

namespace BlazorApp1.Services.Server
{
    public class CharacterPdfService
    {
        public byte[] GenerateCharacterSheet(CharacterDto character)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Text(character.Name)
                            .FontSize(24).Bold().FontColor(Colors.Red.Darken2);
                        col.Item().Text($"{character.Race} • {string.Join(" / ", character.Classes.Select(c => $"{c.Name} {c.Level}"))}")
                            .FontSize(12).FontColor(Colors.Grey.Darken1);
                        col.Item().Text($"Campaign: {character.CampaignName}")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingTop(16).Column(col =>
                    {
                        // Ability Scores
                        col.Item().Text("Ability Scores").FontSize(14).Bold();
                        col.Item().PaddingTop(8).Row(row =>
                        {
                            var abilities = new[]
                            {
                                ("STR", character.Strength),
                                ("DEX", character.Dexterity),
                                ("CON", character.Constitution),
                                ("INT", character.Intelligence),
                                ("WIS", character.Wisdom),
                                ("CHA", character.Charisma),
                            };

                            foreach (var (name, score) in abilities)
                            {
                                var mod = (score - 10) / 2;
                                var modStr = mod >= 0 ? $"+{mod}" : $"{mod}";
                                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(6).Column(c =>
                                    {
                                        c.Item().Text(name).FontSize(9).Bold().AlignCenter();
                                        c.Item().Text(score.ToString()).FontSize(18).Bold().AlignCenter();
                                        c.Item().Text(modStr).FontSize(10).AlignCenter()
                                            .FontColor(Colors.Grey.Darken1);
                                    });
                            }
                        });

                        // Combat Stats
                        col.Item().PaddingTop(16).Text("Combat Stats").FontSize(14).Bold();
                        col.Item().PaddingTop(8).Row(row =>
                        {
                            var stats = new[]
                            {
                                ("AC", character.ArmorClass.ToString()),
                                ("Max HP", character.MaxHitPoints.ToString()),
                                ("Current HP", character.CurrentHitPoints.ToString()),
                                ("Prof Bonus", $"+{character.GetProficiencyBonus()}"),
                                ("Level", character.TotalLevel.ToString()),
                                ("XP", character.ExperiencePoints.ToString()),
                            };

                            foreach (var (label, value) in stats)
                            {
                                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(6).Column(c =>
                                    {
                                        c.Item().Text(label).FontSize(9).Bold().AlignCenter();
                                        c.Item().Text(value).FontSize(16).Bold().AlignCenter();
                                    });
                            }
                        });

                        // Classes
                        col.Item().PaddingTop(16).Text("Classes").FontSize(14).Bold();
                        col.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);
                                cols.RelativeColumn(1);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Text("Class").Bold();
                                header.Cell().Text("Level").Bold();
                            });
                            foreach (var cls in character.Classes)
                            {
                                table.Cell().Text(cls.Name);
                                table.Cell().Text(cls.Level.ToString());
                            }
                        });

                        // Skills — only show proficient ones
                        var proficientSkills = character.Skills?
                            .Where(s => s.IsProficient || s.HasExpertise)
                            .ToList();

                        if (proficientSkills?.Any() == true)
                        {
                            col.Item().PaddingTop(16).Text("Proficiencies & Expertise").FontSize(14).Bold();
                            col.Item().PaddingTop(8).Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(3);
                                    cols.RelativeColumn(2);
                                    cols.RelativeColumn(1);
                                });
                                table.Header(header =>
                                {
                                    header.Cell().Text("Skill").Bold();
                                    header.Cell().Text("Ability").Bold();
                                    header.Cell().Text("Type").Bold();
                                });
                                foreach (var skill in proficientSkills)
                                {
                                    table.Cell().Text(skill.Name);
                                    table.Cell().Text(skill.AbilityScore);
                                    table.Cell().Text(skill.HasExpertise ? "Expertise" : "Proficient");
                                }
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated by DnD Manager • ");
                        x.Span(DateTime.Now.ToString("MMMM dd, yyyy"))
                            .FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }
    }
}