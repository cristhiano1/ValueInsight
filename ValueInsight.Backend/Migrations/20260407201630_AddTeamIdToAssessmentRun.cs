using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValueInsight.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamIdToAssessmentRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "AssessmentRuns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentRuns_TeamId",
                table: "AssessmentRuns",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentRuns_Teams_TeamId",
                table: "AssessmentRuns",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentRuns_Teams_TeamId",
                table: "AssessmentRuns");

            migrationBuilder.DropIndex(
                name: "IX_AssessmentRuns_TeamId",
                table: "AssessmentRuns");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "AssessmentRuns");
        }
    }
}
