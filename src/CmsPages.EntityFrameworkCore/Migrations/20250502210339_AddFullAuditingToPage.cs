using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsPages.Migrations
{
    /// <inheritdoc />
    public partial class AddFullAuditingToPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppPages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppPages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppPages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppPages");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppPages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppPages");
        }
    }
}
