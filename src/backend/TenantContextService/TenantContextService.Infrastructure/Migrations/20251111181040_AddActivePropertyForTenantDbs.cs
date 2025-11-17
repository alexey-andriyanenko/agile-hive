using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantContextService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActivePropertyForTenantDbs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tenants");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "TenantDbs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "TenantDbs");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
