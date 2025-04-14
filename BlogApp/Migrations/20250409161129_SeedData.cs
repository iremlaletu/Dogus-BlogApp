using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Technology" },
                    { 2, "Health" },
                    { 3, "Lifestyle" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Author", "CategoryId", "Content", "CreatedAt", "ImageUrlPath", "Title" },
                values: new object[,]
                {
                    { 1, "Jack", 1, "This is the content of the first post.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://example.com/image1.jpg", "First Post" },
                    { 2, "David", 2, "This is the content of the second post.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://example.com/image2.jpg", "Second Post" },
                    { 3, "Jack", 3, "This is the content of the third post.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://example.com/image3.jpg", "Third Post" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
