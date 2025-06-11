using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStep.EF.Migrations
{
    /// <inheritdoc />
    public partial class indexingForApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Applications_ApplicationTypeID",
                table: "Applications",
                newName: "IX_Application_ApplicationTypeID");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Status",
                table: "Applications",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Application_Status",
                table: "Applications");

            migrationBuilder.RenameIndex(
                name: "IX_Application_ApplicationTypeID",
                table: "Applications",
                newName: "IX_Applications_ApplicationTypeID");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
