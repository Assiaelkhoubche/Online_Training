using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_training.Server.Migrations
{
    /// <inheritdoc />
    public partial class okkkkkkk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string userId = "e7da6428-84f3-4629-be79-b8c151028a06";
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
          keyValues: new object[] { "e7da6428-84f3-4629-be79-b8c151028a06", "6E04FC44-FCCD-42CE-8561-AF057F4B5F7C" });

        }
    }
}
