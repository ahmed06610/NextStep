using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStep.EF.Migrations
{
    /// <inheritdoc />
    public partial class StepEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_ApplicationTypes_ApplicationTypeID",
                table: "Steps");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationTypeID",
                table: "Steps",
                type: "int",
                nullable: true, // Made nullable
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_ApplicationTypes_ApplicationTypeID",
                table: "Steps",
                column: "ApplicationTypeID",
                principalTable: "ApplicationTypes",
                principalColumn: "ApplicationTypeID",
                onDelete: ReferentialAction.SetNull); // Changed from Restrict to SetNull
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_ApplicationTypes_ApplicationTypeID",
                table: "Steps");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationTypeID",
                table: "Steps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_ApplicationTypes_ApplicationTypeID",
                table: "Steps",
                column: "ApplicationTypeID",
                principalTable: "ApplicationTypes",
                principalColumn: "ApplicationTypeID");
        }
    }
}
