using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OstaFeedbackApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMultilingualToQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Questions",
                newName: "TextOr");

            migrationBuilder.AddColumn<string>(
                name: "TextAm",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextAm",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "TextOr",
                table: "Questions",
                newName: "Text");
        }
    }
}
