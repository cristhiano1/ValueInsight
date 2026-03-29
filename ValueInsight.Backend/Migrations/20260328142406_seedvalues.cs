using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ValueInsight.Backend.Migrations
{
    /// <inheritdoc />
    public partial class seedvalues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentValueSelections_Values_ValueId",
                table: "AssessmentValueSelections");

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Engineering" },
                    { 2, "Product" },
                    { 3, "Design" },
                    { 4, "Marketing" },
                    { 5, "Operations" }
                });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BehaviorIndicator", "ShortDefinition" },
                values: new object[] { "Delegates and gives others room.", "I assume others have good intentions." });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BehaviorIndicator", "ShortDefinition" },
                values: new object[] { "Speaks respectfully even in disagreement.", "I treat others with dignity." });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BehaviorIndicator", "Name", "ShortDefinition" },
                values: new object[] { "Listens before reacting.", "Empathy", "I try to understand other perspectives." });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { "Checks in on others regularly.", 1, "Care", "I care about people’s wellbeing." });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { "Invites others into decisions.", 1, "Collaboration", "Shared success matters to me." });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { "Communicates candidly.", 1, "Openness", "I share thoughts and feelings honestly." });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { "Explains reasons behind decisions.", 1, "Transparency", "I am clear about intentions." });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { "Supports the group in tough situations.", 1, "Loyalty", "I stand by my team." });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { "Makes space for quieter voices.", 1, "Inclusion", "Everyone should feel welcome." });

            migrationBuilder.InsertData(
                table: "Values",
                columns: new[] { "Id", "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[,]
                {
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

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentValueSelections_Values_ValueId",
                table: "AssessmentValueSelections",
                column: "ValueId",
                principalTable: "Values",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentValueSelections_Values_ValueId",
                table: "AssessmentValueSelections");

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BehaviorIndicator", "ShortDefinition" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BehaviorIndicator", "ShortDefinition" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BehaviorIndicator", "Name", "ShortDefinition" },
                values: new object[] { null, "Transparency", null });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { null, 2, "Results", null });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { null, 2, "Efficiency", null });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { null, 3, "Structure", null });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { null, 4, "Freedom", null });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { null, 5, "Innovation", null });

            migrationBuilder.UpdateData(
                table: "Values",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "BehaviorIndicator", "Category", "Name", "ShortDefinition" },
                values: new object[] { null, 6, "Purpose", null });

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentValueSelections_Values_ValueId",
                table: "AssessmentValueSelections",
                column: "ValueId",
                principalTable: "Values",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
