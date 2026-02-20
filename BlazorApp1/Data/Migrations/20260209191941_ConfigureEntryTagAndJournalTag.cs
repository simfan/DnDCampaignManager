using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureEntryTagAndJournalTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Entries_EntryId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Journals_JournalId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_EntryId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_JournalId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "EntryId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "JournalId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "EntryTag",
                columns: table => new
                {
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryTag", x => new { x.EntryId, x.TagId });
                    table.ForeignKey(
                        name: "FK_EntryTag_Entries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entries",
                        principalColumn: "EntryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalTag",
                columns: table => new
                {
                    JournalId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalTag", x => new { x.JournalId, x.TagId });
                    table.ForeignKey(
                        name: "FK_JournalTag_Journals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "Journals",
                        principalColumn: "JournalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntryTag_TagId",
                table: "EntryTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalTag_TagId",
                table: "JournalTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryTag");

            migrationBuilder.DropTable(
                name: "JournalTag");

            migrationBuilder.AddColumn<int>(
                name: "EntryId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JournalId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_EntryId",
                table: "Tags",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_JournalId",
                table: "Tags",
                column: "JournalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Entries_EntryId",
                table: "Tags",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "EntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Journals_JournalId",
                table: "Tags",
                column: "JournalId",
                principalTable: "Journals",
                principalColumn: "JournalId");
        }
    }
}
