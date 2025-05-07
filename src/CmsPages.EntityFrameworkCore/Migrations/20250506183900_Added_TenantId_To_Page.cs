using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsPages.Migrations
{
    /// <inheritdoc />
    public partial class Added_TenantId_To_Page : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppPages",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppPages");
        }
    }
}
