using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStep.EF.Migrations
{
    /// <inheritdoc />
    public partial class Adjust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationTypeID",
                table: "Steps",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Steps_ApplicationTypeID",
                table: "Steps",
                column: "ApplicationTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_ApplicationTypes_ApplicationTypeID",
                table: "Steps",
                column: "ApplicationTypeID",
                principalTable: "ApplicationTypes",
                principalColumn: "ApplicationTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_ApplicationTypes_ApplicationTypeID",
                table: "Steps");

            migrationBuilder.DropIndex(
                name: "IX_Steps_ApplicationTypeID",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "ApplicationTypeID",
                table: "Steps");
        }
    }
}
