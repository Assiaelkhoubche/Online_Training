using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_training.Server.Migrations
{
    /// <inheritdoc />
    public partial class progresss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantFormations_Formations_FormationId",
                table: "ParticipantFormations");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantFormations_Participants_ParticipantId",
                table: "ParticipantFormations");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Formations_FormationId",
                table: "Sections");

            migrationBuilder.AlterColumn<string>(
                name: "TrainerId",
                table: "Formations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FormationId = table.Column<int>(type: "int", nullable: false),
                    DateIssued = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DownloadUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Certificates_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FormationId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluations_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionsCompletions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    FormationId = table.Column<int>(type: "int", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionsCompletions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionsCompletions_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectionsCompletions_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectionsCompletions_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Formations_TrainerId",
                table: "Formations",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_FormationId",
                table: "Certificates",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ParticipantId",
                table: "Certificates",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_FormationId",
                table: "Evaluations",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ParticipantId",
                table: "Evaluations",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionsCompletions_FormationId",
                table: "SectionsCompletions",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionsCompletions_ParticipantId",
                table: "SectionsCompletions",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionsCompletions_SectionId",
                table: "SectionsCompletions",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Formations_Trainers_TrainerId",
                table: "Formations",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantFormations_Formations_FormationId",
                table: "ParticipantFormations",
                column: "FormationId",
                principalTable: "Formations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantFormations_Participants_ParticipantId",
                table: "ParticipantFormations",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Formations_FormationId",
                table: "Sections",
                column: "FormationId",
                principalTable: "Formations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formations_Trainers_TrainerId",
                table: "Formations");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantFormations_Formations_FormationId",
                table: "ParticipantFormations");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantFormations_Participants_ParticipantId",
                table: "ParticipantFormations");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Formations_FormationId",
                table: "Sections");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "SectionsCompletions");

            migrationBuilder.DropIndex(
                name: "IX_Formations_TrainerId",
                table: "Formations");

            migrationBuilder.AlterColumn<string>(
                name: "TrainerId",
                table: "Formations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantFormations_Formations_FormationId",
                table: "ParticipantFormations",
                column: "FormationId",
                principalTable: "Formations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantFormations_Participants_ParticipantId",
                table: "ParticipantFormations",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Formations_FormationId",
                table: "Sections",
                column: "FormationId",
                principalTable: "Formations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
