using System.Text;
using System.Text.Json;
using ValueInsight.Backend.Dtos;

namespace ValueInsight.Backend.Services
{
    public class AiCoachService
    {
        private readonly HttpClient _httpClient;

        public AiCoachService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                CoachingRecommendations = new List<string>()
            };

            DetermineAlignmentLevel(response);
            AnalyzeStrengths(response, request.DominantValues);
            GenerateBaseRecommendations(response);

            var aiAdvice = await GenerateAIRecommendations(request, response);

            if (!string.IsNullOrWhiteSpace(aiAdvice))
            {
                response.AICoachingAdvice = aiAdvice;
                response.AIEnhanced = true;
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

            foreach (var value in values)
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
                        response.Strengths.Add("Optimizes workflows and improves productivity.");
                        break;

                    case "Purpose":
                        response.Strengths.Add("Connects work to meaningful organizational goals.");
                        break;

                    default:
                        response.Strengths.Add($"Shows strong alignment with value: {value}");
                        break;
                }
            }
        }

        private void GenerateBaseRecommendations(CoachingResponseDtos response)
        {
            switch (response.AlignmentLevel)
            {
                case "Excellent Alignment":
                    response.CoachingRecommendations.Add("Encourage the individual to mentor others.");
                    response.CoachingRecommendations.Add("Leverage their alignment to strengthen team culture.");
                    break;

                case "Strong Alignment":
                    response.CoachingRecommendations.Add("Provide leadership opportunities.");
                    response.CoachingRecommendations.Add("Encourage participation in cultural initiatives.");
                    break;

                case "Moderate Alignment":
                    response.DevelopmentAreas.Add("Alignment with team values could be improved.");
                    response.CoachingRecommendations.Add("Schedule periodic coaching sessions.");
                    response.CoachingRecommendations.Add("Encourage reflection on personal vs team values.");
                    break;

                case "Low Alignment":
                    response.DevelopmentAreas.Add("Significant cultural misalignment detected.");
                    response.CoachingRecommendations.Add("Conduct structured coaching discussions.");
                    response.CoachingRecommendations.Add("Identify conflicts between personal and team values.");
                    break;

                case "Critical Alignment Risk":
                    response.DevelopmentAreas.Add("Severe cultural misalignment affecting collaboration.");
                    response.CoachingRecommendations.Add("Immediate leadership intervention recommended.");
                    response.CoachingRecommendations.Add("Consider role compatibility within the team.");
                    break;
            }
        }

        private async Task<string?> GenerateAIRecommendations(
            CoachingRequestDtos request,
            CoachingResponseDtos analysis)
        {
            try
            {
                var prompt = $"""
You are an organizational psychologist and leadership coach.

Analyze the cultural alignment of a team member and provide actionable coaching advice.

User Information:
UserId: {request.UserId}
TeamId: {request.TeamId}

Alignment Score: {request.AlignmentScore}
Alignment Level: {analysis.AlignmentLevel}

Dominant Values:
{string.Join(", ", request.DominantValues ?? new List<string>())}

Identified Strengths:
{string.Join(", ", analysis.Strengths)}

Development Areas:
{string.Join(", ", analysis.DevelopmentAreas)}

Provide:
1. A short explanation of the cultural alignment situation.
2. Practical coaching advice.
3. Concrete actions the user can take in the next 30 days.
4. Suggestions for the team leader.

Respond in a professional tone.
""";

                var requestBody = new
                {
                    model = "llama3",
                    prompt = prompt,
                    stream = false
                };

                var json = JsonSerializer.Serialize(requestBody);

                var response = await _httpClient.PostAsync(
                    "http://ollama:11434/api/generate",
                    new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(content);

                return doc.RootElement.GetProperty("response").GetString();
            }
            catch
            {
                return null;
            }
        }

        // =========================
        // ✅ NUEVO — TEAM INSIGHT
        // =========================
        public async Task<string?> GenerateTeamInsightAsync(TeamInsightDtos request)
        {
            try
            {
                var prompt = $"""
You are an organizational psychologist specialized in team culture.

Analyze the following team culture data and provide insights.

Team Culture Type: {request.CultureType}
Alignment Score: {request.AlignmentScore}
Polarization Score: {request.PolarizationScore}
Maturity Index: {request.MaturityIndex}

Top Categories:
{string.Join(", ", request.TopCategories)}

Low Categories:
{string.Join(", ", request.LowCategories)}

Provide:
1. Interpretation of the team culture
2. Risks or tensions in the team
3. Leadership recommendations
4. Actions to improve alignment

Keep it clear and practical.
""";

                return await GenerateRawAsync(prompt);
            }
            catch
            {
                return null;
            }
        }

        // =========================
        // ✅ NUEVO — RAW AI METHOD (FIX ERROR)
        // =========================
        public async Task<string?> GenerateRawAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    model = "llama3",
                    prompt = prompt,
                    stream = false
                };

                var json = JsonSerializer.Serialize(requestBody);

                var response = await _httpClient.PostAsync(
                    "http://ollama:11434/api/generate",
                    new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(content);

                return doc.RootElement.GetProperty("response").GetString();
            }
            catch
            {
                return null;
            }
        }
    }
}