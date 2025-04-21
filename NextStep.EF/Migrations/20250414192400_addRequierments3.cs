using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStep.EF.Migrations
{
    /// <inheritdoc />
    public partial class addRequierments3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequiermentsApplicationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationTypeId = table.Column<int>(type: "int", nullable: false),
                    RequiermentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiermentsApplicationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequiermentsApplicationTypes_ApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalTable: "ApplicationTypes",
                        principalColumn: "ApplicationTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequiermentsApplicationTypes_Requierments_RequiermentId",
                        column: x => x.RequiermentId,
                        principalTable: "Requierments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequiermentsApplicationTypes_ApplicationTypeId",
                table: "RequiermentsApplicationTypes",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequiermentsApplicationTypes_RequiermentId",
                table: "RequiermentsApplicationTypes",
                column: "RequiermentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequiermentsApplicationTypes");
        }
    }
}
