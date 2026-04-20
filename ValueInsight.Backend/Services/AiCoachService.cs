using System.Text;
using System.Text.Json;
using ValueInsight.Backend.Dtos;

namespace ValueInsight.Backend.Services
{
    public class AiCoachService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AiCoachService> _logger;

        public AiCoachService(HttpClient httpClient, IConfiguration configuration, ILogger<AiCoachService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<CoachingResponseDtos> GenerateCoachingAsync(CoachingRequestDtos request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.AlignmentScore < 0 || request.AlignmentScore > 100)
                throw new ArgumentException("Alignment score must be between 0 and 100.");

            var response = new CoachingResponseDtos
            {
                UserId = request.UserId,
                TeamId = request.TeamId,
                AlignmentScore = request.AlignmentScore,
                Strengths = new List<string>(),
                DevelopmentAreas = new List<string>(),
                CoachingRecommendations = new List<string>(),
                GoalSuggestions = new List<string>()
            };

            DetermineAlignmentLevel(response);
            AnalyzeStrengths(response, request.DominantValues);
            GenerateBaseRecommendations(response, request);
            BuildGoalSuggestions(response, request);

            var aiAdvice = await GenerateUserAIRecommendations(request, response);

            if (!string.IsNullOrWhiteSpace(aiAdvice))
            {
                response.AICoachingAdvice = aiAdvice;
                response.AIEnhanced = true;
            }
            else
            {
                response.AICoachingAdvice = BuildDeterministicFallbackAdvice(request, response);
                response.AIEnhanced = false;
            }

            return response;
        }

        public async Task<TeamCoachingResponseDtos> GenerateTeamCoachingAsync(TeamCoachingRequestDtos request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var response = new TeamCoachingResponseDtos
            {
                TeamId = request.TeamId,
                TeamName = request.TeamName,
                CultureType = request.CultureType ?? string.Empty,
                AlignmentScore = request.AlignmentScore,
                PolarizationScore = request.PolarizationScore,
                MaturityIndex = request.MaturityIndex,
                Strengths = BuildTeamStrengths(request),
                Risks = BuildTeamRisks(request),
                LeadershipAdvice = BuildLeadershipAdvice(request),
                SuggestedInterventions = BuildTeamInterventions(request)
            };

            var aiAdvice = await GenerateTeamAIRecommendations(request, response);
            if (!string.IsNullOrWhiteSpace(aiAdvice))
            {
                response.AICoachingAdvice = aiAdvice;
                response.AIEnhanced = true;
            }
            else
            {
                response.AICoachingAdvice = BuildTeamFallbackAdvice(request, response);
                response.AIEnhanced = false;
            }

            return response;
        }

        private void DetermineAlignmentLevel(CoachingResponseDtos response)
        {
            var score = response.AlignmentScore;

            if (score >= 85)
                response.AlignmentLevel = "Excellent Alignment";
            else if (score >= 70)
                response.AlignmentLevel = "Strong Alignment";
            else if (score >= 50)
                response.AlignmentLevel = "Moderate Alignment";
            else if (score >= 30)
                response.AlignmentLevel = "Low Alignment";
            else
                response.AlignmentLevel = "Critical Alignment Risk";
        }

        private void AnalyzeStrengths(CoachingResponseDtos response, List<string>? values)
        {
            if (values == null) return;

            foreach (var value in values.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                switch (value)
                {
                    case "Trust":
                        response.Strengths.Add("Builds strong interpersonal relationships within the team.");
                        break;
                    case "Transparency":
                        response.Strengths.Add("Encourages open communication and information sharing.");
                        break;
                    case "Innovation":
                        response.Strengths.Add("Promotes creative thinking and experimentation.");
                        break;
                    case "Respect":
                        response.Strengths.Add("Fosters psychological safety and inclusion.");
                        break;
                    case "Efficiency":
                    case "Resultatfokus":
                        response.Strengths.Add("Helps the team stay focused on delivery and follow-through.");
                        break;
                    case "Purpose":
                    case "Syfte":
                        response.Strengths.Add("Connects work to meaningful organizational goals.");
                        break;
                    default:
                        response.Strengths.Add($"Shows strong alignment with value: {value}");
                        break;
                }
            }
        }

        private void GenerateBaseRecommendations(CoachingResponseDtos response, CoachingRequestDtos request)
        {
            switch (response.AlignmentLevel)
            {
                case "Excellent Alignment":
                    response.CoachingRecommendations.Add("Use the person's value alignment as a strength in high-trust or change-heavy situations.");
                    response.CoachingRecommendations.Add("Invite them to model team behaviors that reflect the culture you want more of.");
                    break;
                case "Strong Alignment":
                    response.CoachingRecommendations.Add("Create opportunities to take visible ownership in team initiatives.");
                    response.CoachingRecommendations.Add("Use regular reflection to keep the values-to-behavior link active.");
                    break;
                case "Moderate Alignment":
                    response.DevelopmentAreas.Add("Alignment with team values could be improved.");
                    response.CoachingRecommendations.Add("Clarify which team behaviors feel energizing and which create friction.");
                    response.CoachingRecommendations.Add("Use one coaching conversation to compare personal values and team expectations.");
                    break;
                case "Low Alignment":
                    response.DevelopmentAreas.Add("Significant cultural misalignment detected.");
                    response.CoachingRecommendations.Add("Discuss concrete situations where values feel blocked in the current team culture.");
                    response.CoachingRecommendations.Add("Agree on one adaptation strategy and one leader support action.");
                    break;
                case "Critical Alignment Risk":
                    response.DevelopmentAreas.Add("Severe cultural misalignment affecting collaboration.");
                    response.CoachingRecommendations.Add("Address the value mismatch quickly in a structured 1:1 conversation.");
                    response.CoachingRecommendations.Add("Review role expectations, support needs, and possible pressure points.");
                    break;
            }

            if (!string.IsNullOrWhiteSpace(request.CurrentChallenge))
                response.CoachingRecommendations.Add("Use the current challenge as the starting point for the next coaching conversation.");

            if (request.TeamTensionFields.Any())
                response.CoachingRecommendations.Add($"Explore the main team tension field: {request.TeamTensionFields.First()}.");
        }

        private void BuildGoalSuggestions(CoachingResponseDtos response, CoachingRequestDtos request)
        {
            foreach (var value in request.DominantValues.Take(2))
            {
                response.GoalSuggestions.Add($"Create one weekly goal that makes {value} visible in a concrete behavior or team routine.");
            }

            if (!string.IsNullOrWhiteSpace(request.CurrentGoal))
            {
                var linkedValue = !string.IsNullOrWhiteSpace(request.LinkedValue)
                    ? request.LinkedValue!.Trim()
                    : request.DominantValues.FirstOrDefault() ?? "one top value";

                response.GoalSuggestions.Add($"Refine the goal so it clearly supports {linkedValue} and includes one observable behavior or milestone.");

                if (!string.IsNullOrWhiteSpace(request.GoalRationale))
                {
                    response.GoalSuggestions.Add("Use your goal rationale as a check: does the goal still reflect the value you want to lead from?");
                }
            }
            else if (!request.DominantValues.Any())
            {
                response.GoalSuggestions.Add("Write one short goal and link it to at least one of your top values before starting the next work week.");
            }
            else
            {
                response.GoalSuggestions.Add("When you write your next goal, state which top value it supports and what behavior will show progress.");
            }
        }

        private async Task<string?> GenerateUserAIRecommendations(CoachingRequestDtos request, CoachingResponseDtos analysis)
        {
            var userPrompt = BuildCompactUserPrompt(request, analysis);
            return await SendChatPromptAsync(userPrompt);
        }

        private async Task<string?> GenerateTeamAIRecommendations(TeamCoachingRequestDtos request, TeamCoachingResponseDtos analysis)
        {
            var userPrompt = BuildCompactTeamPrompt(request, analysis);
            return await SendChatPromptAsync(userPrompt);
        }

        private async Task<string?> SendChatPromptAsync(string userPrompt)
        {
            try
            {
                var baseUrl = (_configuration["Ollama:BaseUrl"]
                    ?? Environment.GetEnvironmentVariable("OLLAMA_BASE_URL")
                    ?? "http://ollama:11434").TrimEnd('/');

                var model = _configuration["Ollama:Model"]
                    ?? Environment.GetEnvironmentVariable("OLLAMA_MODEL")
                    ?? "phi3:mini";

                var timeoutSeconds = int.TryParse(_configuration["Ollama:TimeoutSeconds"], out var parsedTimeout)
                    ? parsedTimeout
                    : 180;

                _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

                var requestBody = new
                {
                    model,
                    messages = new object[]
                    {
                        new
                        {
                            role = "system",
                            content = "You are a leadership coach and organizational psychologist. Be practical, supportive, and concise."
                        },
                        new
                        {
                            role = "user",
                            content = userPrompt
                        }
                    },
                    stream = false,
                    options = new
                    {
                        temperature = 0.3,
                        num_predict = 350
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var httpResponse = await _httpClient.PostAsync(
                    $"{baseUrl}/api/chat",
                    new StringContent(json, Encoding.UTF8, "application/json"));

                var responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Ollama returned non-success status code {StatusCode}. Body: {Body}", httpResponse.StatusCode, responseBody);
                    return null;
                }

                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("message", out var messageElement) &&
                    messageElement.TryGetProperty("content", out var contentElement))
                {
                    return contentElement.GetString();
                }

                _logger.LogWarning("Ollama response did not contain message.content. Raw body: {Body}", responseBody);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AI coaching generation failed. Falling back to deterministic advice.");
                return null;
            }
        }

        private static string BuildCompactUserPrompt(CoachingRequestDtos request, CoachingResponseDtos analysis)
        {
            var topValues = request.DominantValues.Any() ? string.Join(", ", request.DominantValues.Take(3)) : "Not available";
            var teamTopValues = request.TeamTopValues.Any() ? string.Join(", ", request.TeamTopValues.Take(3)) : "Not available";
            var teamLowestValues = request.TeamLowestValues.Any() ? string.Join(", ", request.TeamLowestValues.Take(3)) : "Not available";
            var tensionFields = request.TeamTensionFields.Any() ? string.Join(", ", request.TeamTensionFields.Take(2)) : "No major tension field detected";
            var reflection = request.ReflectionInsights.Where(x => !string.IsNullOrWhiteSpace(x)).Take(2).ToList();
            var reflectionText = reflection.Any() ? string.Join(" | ", reflection) : "No reflection notes provided.";
            var challenge = string.IsNullOrWhiteSpace(request.CurrentChallenge) ? "No specific challenge was provided." : request.CurrentChallenge.Trim();
            var currentGoal = string.IsNullOrWhiteSpace(request.CurrentGoal) ? "No goal was provided." : request.CurrentGoal.Trim();
            var linkedValue = string.IsNullOrWhiteSpace(request.LinkedValue) ? "No value explicitly linked" : request.LinkedValue.Trim();
            var goalRationale = string.IsNullOrWhiteSpace(request.GoalRationale) ? "No goal rationale provided." : request.GoalRationale.Trim();

            return $"""
Top values: {topValues}
Alignment score: {request.AlignmentScore:0.0}/100
Alignment level: {analysis.AlignmentLevel}
Alignment with team top values: {(request.AlignmentWithTeamTop5?.ToString("0.0") ?? "Not available")}
Team culture: {request.TeamCultureType ?? "Not available"}
Team top values: {teamTopValues}
Lowest team values: {teamLowestValues}
Team tension fields: {tensionFields}
Current challenge: {challenge}
Current goal: {currentGoal}
Linked value: {linkedValue}
Goal rationale: {goalRationale}
Reflection notes: {reflectionText}

Write:
1. Situation summary
2. Likely energy drains
3. Strengths this person brings
4. 3 practical actions for the next 7 days
5. 4 reflective coaching questions
6. One short note to the team leader
7. One goal suggestion linked to one top value

Important:
- Keep it under 350 words.
- Be specific and realistic.
- If no challenge is provided, clearly say the advice is general.
- If a goal is provided, connect the coaching directly to that goal and the linked value.
""";
        }

        private static string BuildCompactTeamPrompt(TeamCoachingRequestDtos request, TeamCoachingResponseDtos analysis)
        {
            var topValues = request.TopValues.Any() ? string.Join(", ", request.TopValues.Take(5)) : "Not available";
            var lowestValues = request.LowestValues.Any() ? string.Join(", ", request.LowestValues.Take(5)) : "Not available";
            var sharedValues = request.SharedCoreValues.Any() ? string.Join(", ", request.SharedCoreValues) : "None clearly shared yet";
            var tensions = request.TensionFields.Any() ? string.Join(", ", request.TensionFields) : "No major tension field detected";

            return $"""
Team: {request.TeamName}
Culture type: {request.CultureType ?? "Not available"}
Alignment score: {request.AlignmentScore:0.0}/100
Polarization score: {request.PolarizationScore:0.0}/100
Maturity index: {request.MaturityIndex:0.0}
Participation: {request.CompletedMembers}/{request.TotalMembers}
Top values: {topValues}
Lowest represented values: {lowestValues}
Shared core values: {sharedValues}
Tension fields: {tensions}

Write:
1. Team culture summary
2. Strengths of the current culture
3. Cultural risks and early warning signals
4. Leadership advice for the next 30 days
5. 3 suggested team interventions or workshop ideas
6. One team goal that should be connected to the culture

Important:
- Keep it under 380 words.
- Be concrete and realistic.
""";
        }

        private static string BuildDeterministicFallbackAdvice(CoachingRequestDtos request, CoachingResponseDtos analysis)
        {
            var dominantValues = request.DominantValues.Any() ? string.Join(", ", request.DominantValues) : "the selected values";
            var teamTopValues = request.TeamTopValues.Any() ? string.Join(", ", request.TeamTopValues) : "the team's current top values";
            var tension = request.TeamTensionFields.FirstOrDefault() ?? "no major tension field has been detected yet";
            var challenge = string.IsNullOrWhiteSpace(request.CurrentChallenge) ? "No explicit challenge was entered." : request.CurrentChallenge.Trim();
            var goalText = string.IsNullOrWhiteSpace(request.CurrentGoal) ? "No explicit goal was entered." : request.CurrentGoal.Trim();
            var linkedValue = string.IsNullOrWhiteSpace(request.LinkedValue) ? (request.DominantValues.FirstOrDefault() ?? "a top value") : request.LinkedValue.Trim();
            var goalRationale = string.IsNullOrWhiteSpace(request.GoalRationale) ? "No goal rationale was entered." : request.GoalRationale.Trim();

            return $"""
Situation summary
You are currently showing {analysis.AlignmentLevel.ToLowerInvariant()} with a score of {request.AlignmentScore:0.0}/100. Your strongest values are {dominantValues}, while the team is currently shaped most by {teamTopValues}.

Likely energy drains
A likely source of friction is: {challenge}. The main team tension field is {tension}. Watch for repeated frustration, withdrawal, or overcompensation in meetings.

Coaching advice for the next 30 days
- Name one concrete situation this week where your top values were supported or blocked.
- In your next 1:1, explain which value matters most and what behavior would help you work better.
- Choose one small experiment for seven days that makes your values more visible in how you communicate or plan work.

Reflection questions
- Which of your top values feels easiest to live out in this team right now?
- Where are you adapting well, and where are you losing energy?
- What support from your leader would reduce the tension you are feeling?

Advice for the team leader
Create space to discuss how the member's values can be used as a strength in the current culture, and clarify one concrete behavior that would improve day-to-day collaboration.
""";
        }

        private static string BuildTeamFallbackAdvice(TeamCoachingRequestDtos request, TeamCoachingResponseDtos analysis)
        {
            var sharedValues = request.SharedCoreValues.Any() ? string.Join(", ", request.SharedCoreValues) : "no clearly shared values yet";
            var tensions = request.TensionFields.Any() ? string.Join(", ", request.TensionFields) : "no major tension field detected";
            var topValues = request.TopValues.Any() ? string.Join(", ", request.TopValues.Take(3)) : "the strongest visible values";

            return $"""
Culture summary
{request.TeamName} currently shows a {request.CultureType} pattern with an alignment score of {request.AlignmentScore:0.0}/100 and a polarization score of {request.PolarizationScore:0.0}/100. The most visible values are {topValues}.

Strengths
The team already has visible shared assets in {sharedValues}. These are useful foundations for collaboration, leadership communication, and role clarity.

Risks and warning signals
The main tension field is {tensions}. Watch for recurring misunderstanding, slower decisions, silence in meetings, or delivery pressure that weakens trust.

Leadership advice
Use the next 30 days to clarify which behaviors the team wants more of, and connect those behaviors to the team's actual values instead of abstract culture language.

Suggested intervention
Run a short workshop where the team defines one behavior to keep, one tension to reduce, and one team goal linked to the culture.
""";
        }

        private static List<string> BuildTeamStrengths(TeamCoachingRequestDtos request)
        {
            var strengths = new List<string>();
            if (request.SharedCoreValues.Any()) strengths.Add("The team shows visible shared values that can anchor collaboration.");
            if (request.AlignmentScore >= 70) strengths.Add("The team has a relatively strong common direction.");
            if (!string.IsNullOrWhiteSpace(request.CultureType)) strengths.Add($"The current culture pattern is recognizable as {request.CultureType}.");
            if (!strengths.Any()) strengths.Add("The team has enough data to start a structured culture conversation.");
            return strengths;
        }

        private static List<string> BuildTeamRisks(TeamCoachingRequestDtos request)
        {
            var risks = new List<string>();
            if (request.PolarizationScore >= 60) risks.Add("Polarization is high enough to create hidden friction or competing expectations.");
            if (request.TensionFields.Any()) risks.Add($"The clearest tension field is {request.TensionFields.First()}.");
            if (request.CompletedMembers < request.TotalMembers) risks.Add("Participation is incomplete, so the culture picture may still shift.");
            if (!risks.Any()) risks.Add("No critical culture risk stands out yet, but watch for drift between values and behavior.");
            return risks;
        }

        private static List<string> BuildLeadershipAdvice(TeamCoachingRequestDtos request)
        {
            return new List<string>
            {
                "Name the two or three team values that should guide decisions this month.",
                "Translate values into one observable team behavior per value.",
                "Use a short team check-in to discuss where current work creates value tension."
            };
        }

        private static List<string> BuildTeamInterventions(TeamCoachingRequestDtos request)
        {
            return new List<string>
            {
                "Run a workshop on how the team wants to perform together, not just what it wants to achieve.",
                "Map one current tension field to specific behaviors that help or hurt collaboration.",
                "Set one team goal and explicitly connect it to one shared core value."
            };
        }
    }
}
