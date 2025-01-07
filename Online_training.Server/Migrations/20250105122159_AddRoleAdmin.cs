using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_training.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
               @"INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
                  VALUES 
                  (NEWID(), 'Administrator', 'ADMINISTRATOR', NEWID())"
           );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
             @"DELETE FROM dbo.AspNetRoles WHERE Name = 'Administrator'"
          );

        }
    }
}
