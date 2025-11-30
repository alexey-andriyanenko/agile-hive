using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionAsPlainText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionAsPlainText",
                table: "Tasks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DescriptionAsPlainText",
                table: "Tasks",
                column: "DescriptionAsPlainText");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_DescriptionAsPlainText",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DescriptionAsPlainText",
                table: "Tasks");
        }
    }
}
