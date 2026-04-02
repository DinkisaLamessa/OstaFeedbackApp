using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OstaFeedbackApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIsEnabledToQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Questions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Questions");
        }
    }
}
