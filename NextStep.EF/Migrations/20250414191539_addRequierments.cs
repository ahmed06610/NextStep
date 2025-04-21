using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStep.EF.Migrations
{
    /// <inheritdoc />
    public partial class addRequierments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByDeptId",
                table: "ApplicationTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTypes_CreatedByDeptId",
                table: "ApplicationTypes",
                column: "CreatedByDeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationTypes_Departments_CreatedByDeptId",
                table: "ApplicationTypes",
                column: "CreatedByDeptId",
                principalTable: "Departments",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationTypes_Departments_CreatedByDeptId",
                table: "ApplicationTypes");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationTypes_CreatedByDeptId",
                table: "ApplicationTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByDeptId",
                table: "ApplicationTypes");
        }
    }
}
