using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NextStep.EF.Migrations
{
    /// <inheritdoc />
    public partial class seedingrequiermntapplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RequiermentsApplicationTypes",
                columns: new[] { "Id", "ApplicationTypeId", "RequiermentId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 2 },
                    { 3, 1, 3 },
                    { 4, 1, 4 },
                    { 5, 2, 1 },
                    { 6, 2, 2 },
                    { 7, 2, 3 },
                    { 8, 2, 4 },
                    { 9, 3, 1 },
                    { 10, 3, 2 },
                    { 11, 3, 3 },
                    { 12, 3, 4 },
                    { 13, 4, 1 },
                    { 14, 4, 2 },
                    { 15, 4, 3 },
                    { 16, 4, 4 },
                    { 17, 17, 5 },
                    { 18, 17, 6 },
                    { 19, 17, 7 },
                    { 20, 18, 5 },
                    { 21, 18, 6 },
                    { 22, 18, 7 },
                    { 23, 19, 5 },
                    { 24, 19, 6 },
                    { 25, 19, 7 },
                    { 26, 20, 5 },
                    { 27, 20, 6 },
                    { 28, 20, 7 },
                    { 29, 21, 8 },
                    { 30, 21, 9 },
                    { 31, 22, 8 },
                    { 32, 22, 9 },
                    { 33, 23, 8 },
                    { 34, 23, 9 },
                    { 35, 24, 8 },
                    { 36, 24, 9 },
                    { 37, 5, 10 },
                    { 38, 5, 11 },
                    { 39, 6, 10 },
                    { 40, 6, 11 },
                    { 41, 7, 10 },
                    { 42, 7, 11 },
                    { 43, 8, 10 },
                    { 44, 8, 11 },
                    { 45, 25, 12 },
                    { 46, 25, 13 },
                    { 47, 25, 14 },
                    { 48, 25, 15 },
                    { 49, 25, 16 },
                    { 50, 25, 17 },
                    { 51, 25, 18 },
                    { 52, 26, 12 },
                    { 53, 26, 13 },
                    { 54, 26, 14 },
                    { 55, 26, 15 },
                    { 56, 26, 16 },
                    { 57, 26, 17 },
                    { 58, 26, 18 },
                    { 59, 27, 12 },
                    { 60, 27, 13 },
                    { 61, 27, 14 },
                    { 62, 27, 15 },
                    { 63, 27, 16 },
                    { 64, 27, 17 },
                    { 65, 27, 18 },
                    { 66, 28, 12 },
                    { 67, 28, 13 },
                    { 68, 28, 14 },
                    { 69, 28, 15 },
                    { 70, 28, 16 },
                    { 71, 28, 17 },
                    { 72, 28, 18 },
                    { 73, 9, 19 },
                    { 74, 9, 20 },
                    { 75, 33, 21 },
                    { 76, 33, 22 },
                    { 77, 33, 23 },
                    { 78, 13, 24 },
                    { 79, 13, 25 },
                    { 80, 13, 26 },
                    { 81, 13, 27 },
                    { 82, 13, 28 },
                    { 83, 13, 29 },
                    { 84, 14, 24 },
                    { 85, 14, 25 },
                    { 86, 14, 26 },
                    { 87, 14, 27 },
                    { 88, 14, 28 },
                    { 89, 14, 29 },
                    { 90, 15, 24 },
                    { 91, 15, 25 },
                    { 92, 15, 26 },
                    { 93, 15, 27 },
                    { 94, 15, 28 },
                    { 95, 15, 29 },
                    { 96, 16, 24 },
                    { 97, 16, 25 },
                    { 98, 16, 26 },
                    { 99, 16, 27 },
                    { 100, 16, 28 },
                    { 101, 16, 29 },
                    { 102, 39, 24 },
                    { 103, 39, 25 },
                    { 104, 39, 26 },
                    { 105, 39, 27 },
                    { 106, 39, 28 },
                    { 107, 39, 29 },
                    { 108, 40, 24 },
                    { 109, 40, 25 },
                    { 110, 40, 26 },
                    { 111, 40, 27 },
                    { 112, 40, 28 },
                    { 113, 40, 29 },
                    { 114, 41, 24 },
                    { 115, 41, 25 },
                    { 116, 41, 26 },
                    { 117, 41, 27 },
                    { 118, 41, 28 },
                    { 119, 41, 29 },
                    { 120, 42, 24 },
                    { 121, 42, 25 },
                    { 122, 42, 26 },
                    { 123, 42, 27 },
                    { 124, 42, 28 },
                    { 125, 42, 29 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "RequiermentsApplicationTypes",
                keyColumn: "Id",
                keyValue: 125);
        }
    }
}
