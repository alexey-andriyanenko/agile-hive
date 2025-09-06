using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganizationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugRemoveDbConstraintsOnOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_organizations_OrganizationId",
                table: "OrganizationMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_organizations",
                table: "organizations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_organization_name_allowed_chars",
                table: "organizations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_organization_name_min_length",
                table: "organizations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_organization_no_double_spaces",
                table: "organizations");

            migrationBuilder.RenameTable(
                name: "organizations",
                newName: "Organizations");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Organizations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Organizations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organizations",
                table: "Organizations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_name",
                table: "Organizations",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_Organizations_OrganizationId",
                table: "OrganizationMembers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_Organizations_OrganizationId",
                table: "OrganizationMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organizations",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_name",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Organizations");

            migrationBuilder.RenameTable(
                name: "Organizations",
                newName: "organizations");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "organizations",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_organizations",
                table: "organizations",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_organization_name_allowed_chars",
                table: "organizations",
                sql: "name ~ '^[A-Za-z0-9 \\-_.]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "ck_organization_name_min_length",
                table: "organizations",
                sql: "char_length(name) >= 2");

            migrationBuilder.AddCheckConstraint(
                name: "ck_organization_no_double_spaces",
                table: "organizations",
                sql: "name !~ ' {2,}'");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_organizations_OrganizationId",
                table: "OrganizationMembers",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
