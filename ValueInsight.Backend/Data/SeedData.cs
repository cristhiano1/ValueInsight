using ValueInsight.Backend.Models;
using ValueInsight.Backend.Services;

namespace ValueInsight.Backend.Data;

public static class SeedData
{
    public static readonly Value[] Values =
    {
        // 1. Relation & Trust
        new Value { Id = 1, Name = "Trust", Category = ValueCategory.RelationAndTrust, ShortDefinition = "I assume others have good intentions.", BehaviorIndicator = "Delegates and gives others room." },
        new Value { Id = 2, Name = "Respect", Category = ValueCategory.RelationAndTrust, ShortDefinition = "I treat others with dignity.", BehaviorIndicator = "Speaks respectfully even in disagreement." },
        new Value { Id = 3, Name = "Empathy", Category = ValueCategory.RelationAndTrust, ShortDefinition = "I try to understand other perspectives.", BehaviorIndicator = "Listens before reacting." },
        new Value { Id = 4, Name = "Care", Category = ValueCategory.RelationAndTrust, ShortDefinition = "I care about people’s wellbeing.", BehaviorIndicator = "Checks in on others regularly." },
        new Value { Id = 5, Name = "Collaboration", Category = ValueCategory.RelationAndTrust, ShortDefinition = "Shared success matters to me.", BehaviorIndicator = "Invites others into decisions." },
        new Value { Id = 6, Name = "Openness", Category = ValueCategory.RelationAndTrust, ShortDefinition = "I share thoughts and feelings honestly.", BehaviorIndicator = "Communicates candidly." },
        new Value { Id = 7, Name = "Transparency", Category = ValueCategory.RelationAndTrust, ShortDefinition = "I am clear about intentions.", BehaviorIndicator = "Explains reasons behind decisions." },
        new Value { Id = 8, Name = "Loyalty", Category = ValueCategory.RelationAndTrust, ShortDefinition = "I stand by my team.", BehaviorIndicator = "Supports the group in tough situations." },
        new Value { Id = 9, Name = "Inclusion", Category = ValueCategory.RelationAndTrust, ShortDefinition = "Everyone should feel welcome.", BehaviorIndicator = "Makes space for quieter voices." },
        new Value { Id = 10, Name = "Fairness", Category = ValueCategory.RelationAndTrust, ShortDefinition = "Decisions should be impartial.", BehaviorIndicator = "Applies standards consistently." },

        // 2. Result & Performance
        new Value { Id = 11, Name = "Results Focus", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "I prioritize delivery.", BehaviorIndicator = "Keeps attention on outcomes." },
        new Value { Id = 12, Name = "Efficiency", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "Resources should be used wisely.", BehaviorIndicator = "Removes unnecessary steps." },
        new Value { Id = 13, Name = "Quality", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "Work should meet a high standard.", BehaviorIndicator = "Reviews details carefully." },
        new Value { Id = 14, Name = "Responsibility", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "I own my commitments.", BehaviorIndicator = "Follows through without reminders." },
        new Value { Id = 15, Name = "Achievement Drive", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "I want to reach ambitious goals.", BehaviorIndicator = "Sets and pursues stretch goals." },
        new Value { Id = 16, Name = "Decisiveness", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "I prefer action over hesitation.", BehaviorIndicator = "Makes timely decisions." },
        new Value { Id = 17, Name = "Clarity", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "Expectations should be clear.", BehaviorIndicator = "Defines ownership and outcomes." },
        new Value { Id = 18, Name = "Discipline", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "I do what is required.", BehaviorIndicator = "Keeps routines and commitments." },
        new Value { Id = 19, Name = "Competitiveness", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "We should strive to be the best.", BehaviorIndicator = "Benchmarks against strong peers." },
        new Value { Id = 20, Name = "Reliability", Category = ValueCategory.ResultAndPerformance, ShortDefinition = "I keep my promises.", BehaviorIndicator = "Delivers consistently." },

        // 3. Structure & Stability
        new Value { Id = 21, Name = "Structure", Category = ValueCategory.StructureAndStability, ShortDefinition = "Clear frameworks are important.", BehaviorIndicator = "Creates process and order." },
        new Value { Id = 22, Name = "Planning", Category = ValueCategory.StructureAndStability, ShortDefinition = "Preparation matters.", BehaviorIndicator = "Plans before acting." },
        new Value { Id = 23, Name = "Predictability", Category = ValueCategory.StructureAndStability, ShortDefinition = "Stability creates safety.", BehaviorIndicator = "Prefers clear expectations." },
        new Value { Id = 24, Name = "Safety", Category = ValueCategory.StructureAndStability, ShortDefinition = "Risks should be minimized.", BehaviorIndicator = "Flags risks early." },
        new Value { Id = 25, Name = "Consistency", Category = ValueCategory.StructureAndStability, ShortDefinition = "Rules should be followed.", BehaviorIndicator = "Applies processes the same way." },
        new Value { Id = 26, Name = "Order", Category = ValueCategory.StructureAndStability, ShortDefinition = "System beats chaos.", BehaviorIndicator = "Keeps work organized." },
        new Value { Id = 27, Name = "Long-Term Thinking", Category = ValueCategory.StructureAndStability, ShortDefinition = "Think several steps ahead.", BehaviorIndicator = "Considers future implications." },
        new Value { Id = 28, Name = "Stability", Category = ValueCategory.StructureAndStability, ShortDefinition = "Avoid unnecessary change.", BehaviorIndicator = "Protects continuity." },
        new Value { Id = 29, Name = "Control", Category = ValueCategory.StructureAndStability, ShortDefinition = "I want oversight.", BehaviorIndicator = "Tracks progress closely." },
        new Value { Id = 30, Name = "Role Clarity", Category = ValueCategory.StructureAndStability, ShortDefinition = "Roles should be clear.", BehaviorIndicator = "Defines responsibilities explicitly." },

        // 4. Autonomy & Freedom
        new Value { Id = 31, Name = "Freedom", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I want room to act.", BehaviorIndicator = "Prefers autonomy in execution." },
        new Value { Id = 32, Name = "Independence", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I make my own decisions.", BehaviorIndicator = "Works well with limited supervision." },
        new Value { Id = 33, Name = "Flexibility", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "Adaptability matters.", BehaviorIndicator = "Adjusts approach quickly." },
        new Value { Id = 34, Name = "Courage", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I dare to take risks.", BehaviorIndicator = "Acts despite uncertainty." },
        new Value { Id = 35, Name = "Integrity", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I stand by my principles.", BehaviorIndicator = "Speaks up when something feels wrong." },
        new Value { Id = 36, Name = "Self-Expression", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I can be myself.", BehaviorIndicator = "Expresses individual style and views." },
        new Value { Id = 37, Name = "Self-Reliance", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I prefer minimal dependence on others.", BehaviorIndicator = "Solves problems independently." },
        new Value { Id = 38, Name = "Initiative", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I act without being told.", BehaviorIndicator = "Starts things proactively." },
        new Value { Id = 39, Name = "Authenticity", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I want to be genuine.", BehaviorIndicator = "Acts in line with own beliefs." },
        new Value { Id = 40, Name = "Agency", Category = ValueCategory.AutonomyAndFreedom, ShortDefinition = "I want power to move things forward.", BehaviorIndicator = "Takes ownership of action." },

        // 5. Development & Innovation
        new Value { Id = 41, Name = "Learning", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "I want to keep developing.", BehaviorIndicator = "Seeks feedback and new knowledge." },
        new Value { Id = 42, Name = "Innovation", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "New ideas matter.", BehaviorIndicator = "Suggests new approaches." },
        new Value { Id = 43, Name = "Creativity", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "I like thinking differently.", BehaviorIndicator = "Generates original ideas." },
        new Value { Id = 44, Name = "Change Readiness", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "I embrace change.", BehaviorIndicator = "Adapts quickly to new conditions." },
        new Value { Id = 45, Name = "Curiosity", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "I explore and ask questions.", BehaviorIndicator = "Investigates before concluding." },
        new Value { Id = 46, Name = "Improvement", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "Small steps forward matter.", BehaviorIndicator = "Looks for continuous improvement." },
        new Value { Id = 47, Name = "Vision", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "I think long-term and forward.", BehaviorIndicator = "Connects today’s work to the future." },
        new Value { Id = 48, Name = "Growth", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "Expansion and development are positive.", BehaviorIndicator = "Looks for opportunities to scale." },
        new Value { Id = 49, Name = "Experimentation", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "Test and adjust.", BehaviorIndicator = "Runs small trials before scaling." },
        new Value { Id = 50, Name = "Challenge", Category = ValueCategory.DevelopmentAndInnovation, ShortDefinition = "I seek stretch.", BehaviorIndicator = "Takes on demanding goals." },

        // 6. Meaning & Purpose
        new Value { Id = 51, Name = "Purpose", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "Work should feel meaningful.", BehaviorIndicator = "Connects work to a larger why." },
        new Value { Id = 52, Name = "Social Responsibility", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "We should contribute to something bigger.", BehaviorIndicator = "Considers societal impact." },
        new Value { Id = 53, Name = "Sustainability", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "Long-term impact matters.", BehaviorIndicator = "Balances present and future needs." },
        new Value { Id = 54, Name = "Ethics", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "Decisions should be morally sound.", BehaviorIndicator = "Raises ethical concerns openly." },
        new Value { Id = 55, Name = "Servant Leadership", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "I support others to succeed.", BehaviorIndicator = "Removes obstacles for others." },
        new Value { Id = 56, Name = "Impact", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "I want to make a difference.", BehaviorIndicator = "Seeks meaningful outcomes." },
        new Value { Id = 57, Name = "Passion", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "I am driven by engagement.", BehaviorIndicator = "Brings visible energy to work." },
        new Value { Id = 58, Name = "Commitment", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "I invest my energy fully.", BehaviorIndicator = "Shows dedication over time." },
        new Value { Id = 59, Name = "Values-Based Action", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "I act in line with my values.", BehaviorIndicator = "Makes choices anchored in principles." },
        new Value { Id = 60, Name = "Balance", Category = ValueCategory.MeaningAndPurpose, ShortDefinition = "Work and life should be sustainable.", BehaviorIndicator = "Protects healthy boundaries." }
    };


    
    
        public static void SeedUsers(ValueInsightDbContext context, PasswordService passwordService)
        {
            // ---------- COACH ----------
            if (!context.Users.Any(u => u.Email == "coach@test.com"))
            {
                var coach = new User
                {
                    Name = "Coach Admin",
                    Email = "coach@test.com",
                    Role = "Coach"
                };

                coach.PasswordHash = passwordService.HashPassword(coach, "1234");

                context.Users.Add(coach);
                context.SaveChanges();
            }

            // ---------- USER ----------
            if (!context.Users.Any(u => u.Email == "user@test.com"))
            {
                var user = new User
                {
                    Name = "Normal User",
                    Email = "user@test.com",
                    Role = "User"
                };

                user.PasswordHash = passwordService.HashPassword(user, "1234");

                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }


