using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ValueInsight.Backend.Migrations
{
    /// <inheritdoc />
    public partial class intialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ShortDefinition = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    BehaviorIndicator = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentRuns_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReflectionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReflectionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReflectionAnswers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    InviteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LeaderId = table.Column<int>(type: "int", nullable: true),
                    AllowPartialReport = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Users_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserValues",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ValueId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserValues", x => new { x.UserId, x.ValueId });
                    table.ForeignKey(
                        name: "FK_UserValues_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserValues_Values_ValueId",
                        column: x => x.ValueId,
                        principalTable: "Values",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentReflectionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentRunId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentReflectionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentReflectionAnswers_AssessmentRuns_AssessmentRunId",
                        column: x => x.AssessmentRunId,
                        principalTable: "AssessmentRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentValueSelections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentRunId = table.Column<int>(type: "int", nullable: false),
                    ValueId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentValueSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentValueSelections_AssessmentRuns_AssessmentRunId",
                        column: x => x.AssessmentRunId,
                        principalTable: "AssessmentRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentValueSelections_Values_ValueId",
                        column: x => x.ValueId,
                        principalTable: "Values",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CulturalFitResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    AlignmentScore = table.Column<double>(type: "float", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CulturalFitResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CulturalFitResults_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamJoinRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<int>(type: "int", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamJoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamJoinRequests_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamJoinRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HasCompletedAssessment = table.Column<bool>(type: "bit", nullable: false),
                    JoinedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "AllowPartialReport", "InviteCode", "LeaderId", "Name" },
                values: new object[,]
                {
                    { 1, false, "ENG001", null, "Engineering" },
                    { 2, false, "PROD001", null, "Product" },
                    { 3, false, "DES001", null, "Design" },
                    { 4, false, "MKT001", null, "Marketing" },
                    { 5, false, "OPS001", null, "Operations" }
                });

            migrationBuilder.InsertData(
                table: "Values",
                columns: new[] { "Id", "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[,]
                {
                    { 1, "Delegates and gives others room.", 1, "Trust", "I assume others have good intentions." },
                    { 2, "Speaks respectfully even in disagreement.", 1, "Respect", "I treat others with dignity." },
                    { 3, "Listens before reacting.", 1, "Empathy", "I try to understand other perspectives." },
                    { 4, "Checks in on others regularly.", 1, "Care", "I care about people’s wellbeing." },
                    { 5, "Invites others into decisions.", 1, "Collaboration", "Shared success matters to me." },
                    { 6, "Communicates candidly.", 1, "Openness", "I share thoughts and feelings honestly." },
                    { 7, "Explains reasons behind decisions.", 1, "Transparency", "I am clear about intentions." },
                    { 8, "Supports the group in tough situations.", 1, "Loyalty", "I stand by my team." },
                    { 9, "Makes space for quieter voices.", 1, "Inclusion", "Everyone should feel welcome." },
                    { 10, "Applies standards consistently.", 1, "Fairness", "Decisions should be impartial." },
                    { 11, "Keeps attention on outcomes.", 2, "Results Focus", "I prioritize delivery." },
                    { 12, "Removes unnecessary steps.", 2, "Efficiency", "Resources should be used wisely." },
                    { 13, "Reviews details carefully.", 2, "Quality", "Work should meet a high standard." },
                    { 14, "Follows through without reminders.", 2, "Responsibility", "I own my commitments." },
                    { 15, "Sets and pursues stretch goals.", 2, "Achievement Drive", "I want to reach ambitious goals." },
                    { 16, "Makes timely decisions.", 2, "Decisiveness", "I prefer action over hesitation." },
                    { 17, "Defines ownership and outcomes.", 2, "Clarity", "Expectations should be clear." },
                    { 18, "Keeps routines and commitments.", 2, "Discipline", "I do what is required." },
                    { 19, "Benchmarks against strong peers.", 2, "Competitiveness", "We should strive to be the best." },
                    { 20, "Delivers consistently.", 2, "Reliability", "I keep my promises." },
                    { 21, "Creates process and order.", 3, "Structure", "Clear frameworks are important." },
                    { 22, "Plans before acting.", 3, "Planning", "Preparation matters." },
                    { 23, "Prefers clear expectations.", 3, "Predictability", "Stability creates safety." },
                    { 24, "Flags risks early.", 3, "Safety", "Risks should be minimized." },
                    { 25, "Applies processes the same way.", 3, "Consistency", "Rules should be followed." },
                    { 26, "Keeps work organized.", 3, "Order", "System beats chaos." },
                    { 27, "Considers future implications.", 3, "Long-Term Thinking", "Think several steps ahead." },
                    { 28, "Protects continuity.", 3, "Stability", "Avoid unnecessary change." },
                    { 29, "Tracks progress closely.", 3, "Control", "I want oversight." },
                    { 30, "Defines responsibilities explicitly.", 3, "Role Clarity", "Roles should be clear." },
                    { 31, "Prefers autonomy in execution.", 4, "Freedom", "I want room to act." },
                    { 32, "Works well with limited supervision.", 4, "Independence", "I make my own decisions." },
                    { 33, "Adjusts approach quickly.", 4, "Flexibility", "Adaptability matters." },
                    { 34, "Acts despite uncertainty.", 4, "Courage", "I dare to take risks." },
                    { 35, "Speaks up when something feels wrong.", 4, "Integrity", "I stand by my principles." },
                    { 36, "Expresses individual style and views.", 4, "Self-Expression", "I can be myself." },
                    { 37, "Solves problems independently.", 4, "Self-Reliance", "I prefer minimal dependence on others." },
                    { 38, "Starts things proactively.", 4, "Initiative", "I act without being told." },
                    { 39, "Acts in line with own beliefs.", 4, "Authenticity", "I want to be genuine." },
                    { 40, "Takes ownership of action.", 4, "Agency", "I want power to move things forward." },
                    { 41, "Seeks feedback and new knowledge.", 5, "Learning", "I want to keep developing." },
                    { 42, "Suggests new approaches.", 5, "Innovation", "New ideas matter." },
                    { 43, "Generates original ideas.", 5, "Creativity", "I like thinking differently." },
                    { 44, "Adapts quickly to new conditions.", 5, "Change Readiness", "I embrace change." },
                    { 45, "Investigates before concluding.", 5, "Curiosity", "I explore and ask questions." },
                    { 46, "Looks for continuous improvement.", 5, "Improvement", "Small steps forward matter." },
                    { 47, "Connects today’s work to the future.", 5, "Vision", "I think long-term and forward." },
                    { 48, "Looks for opportunities to scale.", 5, "Growth", "Expansion and development are positive." },
                    { 49, "Runs small trials before scaling.", 5, "Experimentation", "Test and adjust." },
                    { 50, "Takes on demanding goals.", 5, "Challenge", "I seek stretch." },
                    { 51, "Connects work to a larger why.", 6, "Purpose", "Work should feel meaningful." },
                    { 52, "Considers societal impact.", 6, "Social Responsibility", "We should contribute to something bigger." },
                    { 53, "Balances present and future needs.", 6, "Sustainability", "Long-term impact matters." },
                    { 54, "Raises ethical concerns openly.", 6, "Ethics", "Decisions should be morally sound." },
                    { 55, "Removes obstacles for others.", 6, "Servant Leadership", "I support others to succeed." },
                    { 56, "Seeks meaningful outcomes.", 6, "Impact", "I want to make a difference." },
                    { 57, "Brings visible energy to work.", 6, "Passion", "I am driven by engagement." },
                    { 58, "Shows dedication over time.", 6, "Commitment", "I invest my energy fully." },
                    { 59, "Makes choices anchored in principles.", 6, "Values-Based Action", "I act in line with my values." },
                    { 60, "Protects healthy boundaries.", 6, "Balance", "Work and life should be sustainable." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentReflectionAnswers_AssessmentRunId",
                table: "AssessmentReflectionAnswers",
                column: "AssessmentRunId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentRuns_UserId",
                table: "AssessmentRuns",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentValueSelections_AssessmentRunId",
                table: "AssessmentValueSelections",
                column: "AssessmentRunId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentValueSelections_ValueId",
                table: "AssessmentValueSelections",
                column: "ValueId");

            migrationBuilder.CreateIndex(
                name: "IX_CulturalFitResults_TeamId",
                table: "CulturalFitResults",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ReflectionAnswers_UserId",
                table: "ReflectionAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamJoinRequests_TeamId",
                table: "TeamJoinRequests",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamJoinRequests_UserId",
                table: "TeamJoinRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId_UserId",
                table: "TeamMembers",
                columns: new[] { "TeamId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId",
                table: "TeamMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_InviteCode",
                table: "Teams",
                column: "InviteCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeaderId",
                table: "Teams",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserValues_ValueId",
                table: "UserValues",
                column: "ValueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentReflectionAnswers");

            migrationBuilder.DropTable(
                name: "AssessmentValueSelections");

            migrationBuilder.DropTable(
                name: "CulturalFitResults");

            migrationBuilder.DropTable(
                name: "ReflectionAnswers");

            migrationBuilder.DropTable(
                name: "TeamJoinRequests");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "UserValues");

            migrationBuilder.DropTable(
                name: "AssessmentRuns");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
