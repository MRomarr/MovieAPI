using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class addRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b4a2bf0a-074f-4b60-a950-c1d3e10e028e", "7b9cf762-f7d3-400f-9b24-b62f0bfda476", "Admin", "ADMIN" },
                    { "ff1166a6-6a4a-416e-8300-a70e3bfe2709", "bdc4ff4d-97bf-40d3-a318-e024a4b1d107", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b4a2bf0a-074f-4b60-a950-c1d3e10e028e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ff1166a6-6a4a-416e-8300-a70e3bfe2709");
        }
    }
}
