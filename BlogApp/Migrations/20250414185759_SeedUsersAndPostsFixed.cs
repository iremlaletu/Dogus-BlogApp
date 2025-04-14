using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersAndPostsFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "11111111-aaaa-bbbb-cccc-222222222222", 0, "seeded-user1-concurrencystamp", "demo@example.com", true, false, null, "DEMO@EXAMPLE.COM", "DEMO@EXAMPLE.COM", "AQAAAAEAACcQAAAAEMOCKEXAMPLEPASSWORDHASH==", null, false, "seeded-user1-securitystamp", false, "demo@example.com" },
                    { "22222222-aaaa-bbbb-cccc-333333333333", 0, "seeded-user1-concurrencystamp", "user2@example.com", true, false, null, "USER2@EXAMPLE.COM", "USER2@EXAMPLE.COM", "AQAAAAEAACcQAAAAEMOCKEXAMPLEPASSWORDHASH==", null, false, "seeded-user1-securitystamp", false, "user2@example.com" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Author", "CategoryId", "Content", "CreatedAt", "ImageUrlPath", "Title", "UserId" },
                values: new object[,]
                {
                    { 99, "demo@example.com", 1, "This is a seeded post by demo@example.com.", new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://placehold.co/600x400?text=Post1", "Post by Demo User", "11111111-aaaa-bbbb-cccc-222222222222" },
                    { 100, "user2@example.com", 2, "This is a seeded post by user2@example.com.", new DateTime(2025, 4, 2, 0, 0, 0, 0, DateTimeKind.Utc), "https://placehold.co/600x400?text=Post2", "Post by Second User", "22222222-aaaa-bbbb-cccc-333333333333" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "AuthorName", "Content", "CreatedAt", "PostId", "UserId" },
                values: new object[] { 100, "demo@example.com", "This is a seeded comment from demo user on user2's post.", new DateTime(2025, 4, 3, 0, 0, 0, 0, DateTimeKind.Utc), 100, "11111111-aaaa-bbbb-cccc-222222222222" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "11111111-aaaa-bbbb-cccc-222222222222");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "22222222-aaaa-bbbb-cccc-333333333333");
        }
    }
}
