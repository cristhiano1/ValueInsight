using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValueInsight.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToAssessmentValueSelection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AssessmentValueSelections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AssessmentValueSelections");
        }
    }
}
