﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeTagsTenantWide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name_TenantId_ProjectId",
                table: "Tags");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Tags",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_TenantId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TenantId_ProjectId_Name",
                table: "Tags");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Tags",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name_TenantId_ProjectId",
                table: "Tags",
                columns: new[] { "Name", "TenantId", "ProjectId" },
                unique: true);
        }
    }
}
