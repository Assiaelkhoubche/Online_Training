using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_training.Server.Migrations
{
    /// <inheritdoc />
    public partial class stripe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Paniers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "participantSecret",
                table: "Paniers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Paniers");

            migrationBuilder.DropColumn(
                name: "participantSecret",
                table: "Paniers");
        }
    }
}
