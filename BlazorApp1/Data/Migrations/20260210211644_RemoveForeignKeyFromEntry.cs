using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForeignKeyFromEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_AspNetUsers_AuthorId",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_AuthorId",
                table: "Entries");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Entries",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Entries",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_AuthorId",
                table: "Entries",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_AspNetUsers_AuthorId",
                table: "Entries",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
