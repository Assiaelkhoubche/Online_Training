using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_training.Server.Migrations
{
    /// <inheritdoc />
    public partial class stringId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paniers_Participants_ParticipantId1",
                table: "Paniers");

            migrationBuilder.DropIndex(
                name: "IX_Paniers_ParticipantId1",
                table: "Paniers");

            migrationBuilder.DropColumn(
                name: "ParticipantId1",
                table: "Paniers");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "Paniers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Paniers_ParticipantId",
                table: "Paniers",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Paniers_Participants_ParticipantId",
                table: "Paniers",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paniers_Participants_ParticipantId",
                table: "Paniers");

            migrationBuilder.DropIndex(
                name: "IX_Paniers_ParticipantId",
                table: "Paniers");

            migrationBuilder.AlterColumn<int>(
                name: "ParticipantId",
                table: "Paniers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ParticipantId1",
                table: "Paniers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Paniers_ParticipantId1",
                table: "Paniers",
                column: "ParticipantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Paniers_Participants_ParticipantId1",
                table: "Paniers",
                column: "ParticipantId1",
                principalTable: "Participants",
                principalColumn: "Id");
        }
    }
}
