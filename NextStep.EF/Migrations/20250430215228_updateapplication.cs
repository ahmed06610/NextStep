using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStep.EF.Migrations
{
    /// <inheritdoc />
    public partial class updateapplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_ApplicationTypes_ApplicationTypeID",
                table: "Steps");

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "Applications",
                type: "bit",
                nullable: false,
                defaultValue: false);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_ApplicationTypes_ApplicationTypeID",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "Applications");

         
        }
    }
}
