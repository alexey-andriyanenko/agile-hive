using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganizationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToExistingOrganizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Organizations""
                SET ""Slug"" = LOWER(REGEXP_REPLACE(TRIM(""name""), ' +', '-', 'g'))
                WHERE ""Slug"" IS NULL OR ""Slug"" = '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No need to revert the slug generation
        }
    }
}
