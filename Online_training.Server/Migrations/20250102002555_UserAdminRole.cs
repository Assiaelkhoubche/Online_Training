using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_training.Server.Migrations
{
    /// <inheritdoc />
    public partial class UserAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            string userId = "cb20caab-776b-45c0-b7ad-836d1c204d33";
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
           keyValues: new object[] { "cb20caab-776b-45c0-b7ad-836d1c204d33", "6E04FC44-FCCD-42CE-8561-AF057F4B5F7C" });

        }
    }
}
