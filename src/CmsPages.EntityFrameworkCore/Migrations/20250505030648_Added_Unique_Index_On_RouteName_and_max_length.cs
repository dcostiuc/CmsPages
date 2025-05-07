using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsPages.Migrations
{
    /// <inheritdoc />
    public partial class Added_Unique_Index_On_RouteName_and_max_length : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RouteName",
                table: "AppPages",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_AppPages_RouteName",
                table: "AppPages",
                column: "RouteName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppPages_RouteName",
                table: "AppPages");

            migrationBuilder.AlterColumn<string>(
                name: "RouteName",
                table: "AppPages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);
        }
    }
}
