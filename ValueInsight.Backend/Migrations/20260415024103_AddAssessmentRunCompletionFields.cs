using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValueInsight.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAssessmentRunCompletionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "AssessmentRuns",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "AssessmentRuns");
        }
    }
}
