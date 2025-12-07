using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_TenantId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TenantId_ProjectId_Name",
                table: "Tags");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Id_TenantId_Name",
                table: "Tags",
                columns: new[] { "Id", "TenantId", "Name" },
                unique: true,
                filter: "\"ProjectId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Id_TenantId_ProjectId_Name",
                table: "Tags",
                columns: new[] { "Id", "TenantId", "ProjectId", "Name" },
                unique: true,
                filter: "\"ProjectId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Id_TenantId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Id_TenantId_ProjectId_Name",
                table: "Tags");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TenantId_Name",
                table: "Tags",
                columns: new[] { "TenantId", "Name" },
                unique: true,
                filter: "\"ProjectId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TenantId_ProjectId_Name",
                table: "Tags",
                columns: new[] { "TenantId", "ProjectId", "Name" },
                unique: true,
                filter: "\"ProjectId\" IS NOT NULL");
        }
    }
}
