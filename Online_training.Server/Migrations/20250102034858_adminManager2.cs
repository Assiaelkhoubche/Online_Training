using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_training.Server.Migrations
{
    /// <inheritdoc />
    public partial class adminManager2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string userId = "dbfca131-5a54-4776-bded-10d17d859f51";
            migrationBuilder.InsertData(
            table: "AspNetUserRoles",
            columns: new[] { "UserId", "RoleId" },
            values: new object[] { userId, "6E04FC44-FCCD-42CE-8561-AF057F4B5F7C" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
           table: "AspNetUserRoles",
           keyColumns: new[] { "UserId", "RoleId" },
           keyValues: new object[] { "dbfca131-5a54-4776-bded-10d17d859f51", "6E04FC44-FCCD-42CE-8561-AF057F4B5F7C" });

        }
    }
}
