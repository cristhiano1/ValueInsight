//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;
//using Microsoft.AspNetCore.Mvc;
//using ValueInsight.Frontend.Models;

//namespace ValueInsight.Frontend.Controllers;

//public class ReportsController : Controller
//{

//    private static PersonalReportViewModel NormalizePersonalReport(PersonalReportViewModel model)
//    {
//        model.TopValues ??= new();
//        model.CategoryProfile ??= new();
//        model.ReflectionInsights ??= new();
//        model.AssessmentHistory ??= new();
//        foreach (var item in model.AssessmentHistory)
//        {
//            item.TopValues ??= new();
//            item.CategoryProfile ??= new();
//            item.ReflectionInsights ??= new();
//            item.ReflectionQuestions ??= new();
//        }
//        model.ValueConflicts ??= new();
//        model.CoachingQuestions ??= new();
//        return model;
//    }

//    private static TeamReportViewModel NormalizeTeamReport(TeamReportViewModel model)
//    {
//        model.CategoryProfile ??= new();
//        model.TopValues ??= new();
//        model.LowestValues ??= new();
//        model.ValueFrequency ??= new();
//        model.SharedCoreValues ??= new();
//        model.TensionFields ??= new();
//        model.History ??= new();
//        model.Members ??= new();
//        model.HistorySummary ??= model.HistorySummary ?? new TeamHistorySummaryViewModel();
//        model.HistorySummary.PreviousTopValues ??= new();
//        model.HistorySummary.CurrentTopValues ??= new();
//        return model;
//    }

//    private static CoachingResponseViewModel NormalizeCoachingResponse(CoachingResponseViewModel model)
//    {
//        model.Strengths ??= new();
//        model.DevelopmentAreas ??= new();
//        model.CoachingRecommendations ??= new();
//        model.GoalSuggestions ??= new();
//        return model;
//    }

//    private static TeamCoachingResponseViewModel NormalizeTeamCoachingResponse(TeamCoachingResponseViewModel model)
//    {
//        model.Strengths ??= new();
//        model.Risks ??= new();
//        model.LeadershipAdvice ??= new();
//        model.SuggestedInterventions ??= new();
//        return model;
//    }
//    private readonly IHttpClientFactory _httpClientFactory;
//    private readonly IConfiguration _configuration;

//    public ReportsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
//    {
//        _httpClientFactory = httpClientFactory;
//        _configuration = configuration;
//    }

//    private HttpClient CreateAuthorizedClient()
//    {
//        var token = HttpContext.Session.GetString("JWToken");
//        var client = _httpClientFactory.CreateClient();
//        client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);
//        client.Timeout = TimeSpan.FromMinutes(3);
//        if (!string.IsNullOrWhiteSpace(token))
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//        return client;
//    }

//    [HttpGet]
//    public async Task<IActionResult> Personal()
//    {
//        var client = CreateAuthorizedClient();
//        var response = await client.GetAsync("/api/reports/me");
//        if (!response.IsSuccessStatusCode)
//            return RedirectToAction("Login", "Account");

//        var json = await response.Content.ReadAsStringAsync();
//        var model = JsonSerializer.Deserialize<PersonalReportViewModel>(json, new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true
//        }) ?? new PersonalReportViewModel();

//        return View(NormalizePersonalReport(model));
//    }

//    [HttpGet]
//    public async Task<IActionResult> CoachingPrompts(string? currentChallenge, string? currentGoal, string? linkedValue, string? goalRationale)
//    {
//        var client = CreateAuthorizedClient();
//        var reportResponse = await client.GetAsync("/api/reports/me");
//        if (!reportResponse.IsSuccessStatusCode)
//            return RedirectToAction("Login", "Account");

//        var reportJson = await reportResponse.Content.ReadAsStringAsync();
//        var personalReport = JsonSerializer.Deserialize<PersonalReportViewModel>(reportJson, new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true
//        }) ?? new PersonalReportViewModel();
//        personalReport = NormalizePersonalReport(personalReport);

//        var pageModel = new CoachingPromptsPageViewModel
//        {
//            PersonalReport = personalReport,
//            CurrentChallenge = currentChallenge,
//            CurrentGoal = currentGoal,
//            LinkedValue = linkedValue,
//            GoalRationale = goalRationale
//        };

//        if (!personalReport.TeamId.HasValue)
//        {
//            pageModel.ErrorMessage = "Join a team to generate AI coaching prompts.";
//            return View(pageModel);
//        }

//        TeamReportViewModel? teamReport = null;
//        var teamResponse = await client.GetAsync($"/api/reports/team/{personalReport.TeamId.Value}");
//        if (teamResponse.IsSuccessStatusCode)
//        {
//            var teamJson = await teamResponse.Content.ReadAsStringAsync();
//            teamReport = JsonSerializer.Deserialize<TeamReportViewModel>(teamJson, new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            });
//            if (teamReport != null) teamReport = NormalizeTeamReport(teamReport);
//        }

//        var coachingRequest = new CoachingRequestViewModel
//        {
//            UserId = personalReport.UserId,
//            TeamId = personalReport.TeamId.Value,
//            AlignmentScore = personalReport.CulturalFitScore ?? personalReport.AlignmentWithTeamTop5 ?? 0,
//            AlignmentWithTeamTop5 = personalReport.AlignmentWithTeamTop5,
//            TeamCultureType = personalReport.TeamCultureType,
//            CurrentChallenge = currentChallenge,
//            CurrentGoal = currentGoal,
//            LinkedValue = linkedValue,
//            GoalRationale = goalRationale,
//            DominantValues = personalReport.TopValues.OrderBy(x => x.Rank).Take(3).Select(x => x.Name).ToList(),
//            TeamTopValues = teamReport?.TopValues.OrderBy(x => x.Rank).Take(5).Select(x => x.Name).ToList() ?? new List<string>(),
//            TeamLowestValues = teamReport?.LowestValues.Select(x => x.Name).ToList() ?? new List<string>(),
//            TeamTensionFields = teamReport?.TensionFields ?? new List<string>(),
//            ReflectionInsights = personalReport.ReflectionInsights.SelectMany(x => new[] { x.Meaning, x.Behavior, x.Friction }).Where(x => !string.IsNullOrWhiteSpace(x)).ToList()
//        };

//        var coachingJson = JsonSerializer.Serialize(coachingRequest);
//        var coachingResponse = await client.PostAsync(
//            "/api/AiCoach/generate",
//            new StringContent(coachingJson, Encoding.UTF8, "application/json"));

//        if (coachingResponse.IsSuccessStatusCode)
//        {
//            var coachingContent = await coachingResponse.Content.ReadAsStringAsync();
//            pageModel.CoachingResult = JsonSerializer.Deserialize<CoachingResponseViewModel>(coachingContent, new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            });
//            if (pageModel.CoachingResult != null) pageModel.CoachingResult = NormalizeCoachingResponse(pageModel.CoachingResult);
//        }
//        else
//        {
//            pageModel.ErrorMessage = "AI coaching prompts could not be generated right now. You can still use the suggested reflection prompts below.";
//        }

//        return View(pageModel);
//    }

//    [HttpGet]
//    public async Task<IActionResult> Team(int teamId)
//    {
//        var client = CreateAuthorizedClient();
//        var response = await client.GetAsync($"/api/reports/team/{teamId}");
//        if (!response.IsSuccessStatusCode)
//            return RedirectToAction(nameof(Personal));

//        var json = await response.Content.ReadAsStringAsync();
//        var model = JsonSerializer.Deserialize<TeamReportViewModel>(json, new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true
//        }) ?? new TeamReportViewModel();

//        return View(NormalizeTeamReport(model));
//    }

//    [HttpGet]
//    public async Task<IActionResult> TeamCoaching(int teamId)
//    {
//        var client = CreateAuthorizedClient();
//        var teamResponse = await client.GetAsync($"/api/reports/team/{teamId}");
//        if (!teamResponse.IsSuccessStatusCode)
//            return RedirectToAction(nameof(Personal));

//        var teamJson = await teamResponse.Content.ReadAsStringAsync();
//        var teamReport = JsonSerializer.Deserialize<TeamReportViewModel>(teamJson, new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true
//        }) ?? new TeamReportViewModel();
//        teamReport = NormalizeTeamReport(teamReport);

//        var pageModel = new TeamCoachingPageViewModel
//        {
//            TeamReport = teamReport
//        };

//        var coachingRequest = new TeamCoachingRequestViewModel
//        {
//            TeamId = teamReport.TeamId,
//            TeamName = teamReport.TeamName,
//            CultureType = teamReport.CultureType,
//            AlignmentScore = teamReport.AlignmentScore,
//            PolarizationScore = teamReport.PolarizationScore,
//            MaturityIndex = teamReport.MaturityIndex,
//            TeamSize = teamReport.TeamSize,
//            CompletedMembers = teamReport.CompletedMembers,
//            TotalMembers = teamReport.TotalMembers,
//            TopValues = teamReport.TopValues.OrderByDescending(x => x.Rank).Take(5).Select(x => x.Name).ToList(),
//            LowestValues = teamReport.LowestValues.Select(x => x.Name).ToList(),
//            SharedCoreValues = teamReport.SharedCoreValues,
//            TensionFields = teamReport.TensionFields
//        };

//        var coachingJson = JsonSerializer.Serialize(coachingRequest);
//        var coachingResponse = await client.PostAsync(
//            "/api/AiCoach/generate-team",
//            new StringContent(coachingJson, Encoding.UTF8, "application/json"));

//        if (coachingResponse.IsSuccessStatusCode)
//        {
//            var coachingContent = await coachingResponse.Content.ReadAsStringAsync();
//            pageModel.CoachingResult = JsonSerializer.Deserialize<TeamCoachingResponseViewModel>(coachingContent, new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            });
//            if (pageModel.CoachingResult != null) pageModel.CoachingResult = NormalizeTeamCoachingResponse(pageModel.CoachingResult);
//        }
//        else
//        {
//            pageModel.ErrorMessage = "Team AI coaching could not be generated right now. Review the team report data and try again.";
//        }

//        return View(pageModel);
//    }
//}

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ValueInsight.Frontend.Models;

namespace ValueInsight.Frontend.Controllers;

public class ReportsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ReportsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    private static PersonalReportViewModel NormalizePersonalReport(PersonalReportViewModel model)
    {
        model.TopValues ??= new();
        model.CategoryProfile ??= new();
        model.ReflectionInsights ??= new();
        model.AssessmentHistory ??= new();

        foreach (var item in model.AssessmentHistory)
        {
            item.TopValues ??= new();
            item.CategoryProfile ??= new();
            item.ReflectionInsights ??= new();
            item.ReflectionQuestions ??= new();
        }

        model.ValueConflicts ??= new();
        model.CoachingQuestions ??= new();
        return model;
    }

    private static TeamReportViewModel NormalizeTeamReport(TeamReportViewModel model)
    {
        model.CategoryProfile ??= new();
        model.TopValues ??= new();
        model.LowestValues ??= new();
        model.ValueFrequency ??= new();
        model.SharedCoreValues ??= new();
        model.TensionFields ??= new();
        model.History ??= new();
        model.Members ??= new();
        model.HistorySummary ??= new TeamHistorySummaryViewModel();
        model.HistorySummary.PreviousTopValues ??= new();
        model.HistorySummary.CurrentTopValues ??= new();

        return model;
    }

    private static CoachingResponseViewModel NormalizeCoachingResponse(CoachingResponseViewModel model)
    {
        model.Strengths ??= new();
        model.DevelopmentAreas ??= new();
        model.CoachingRecommendations ??= new();
        model.GoalSuggestions ??= new();
        return model;
    }

    private static TeamCoachingResponseViewModel NormalizeTeamCoachingResponse(TeamCoachingResponseViewModel model)
    {
        model.Strengths ??= new();
        model.Risks ??= new();
        model.LeadershipAdvice ??= new();
        model.SuggestedInterventions ??= new();
        return model;
    }

    private HttpClient CreateAuthorizedClient()
    {
        var token = HttpContext.Session.GetString("JWToken");

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);
        client.Timeout = TimeSpan.FromMinutes(3);

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    [HttpGet]
    public async Task<IActionResult> Personal()
    {
        var client = CreateAuthorizedClient();

        var response = await client.GetAsync("/api/reports/me");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Login", "Account");

        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<PersonalReportViewModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new PersonalReportViewModel();

        return View(NormalizePersonalReport(model));
    }

    [HttpGet]
    public async Task<IActionResult> CoachingPrompts(
        string? currentChallenge,
        string? currentGoal,
        string? linkedValue,
        string? goalRationale,
        string mode = "ai")
    {
        var client = CreateAuthorizedClient();

        var reportResponse = await client.GetAsync("/api/reports/me");
        if (!reportResponse.IsSuccessStatusCode)
            return RedirectToAction("Login", "Account");

        var reportJson = await reportResponse.Content.ReadAsStringAsync();

        var personalReport = JsonSerializer.Deserialize<PersonalReportViewModel>(reportJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new PersonalReportViewModel();

        personalReport = NormalizePersonalReport(personalReport);

        var pageModel = new CoachingPromptsPageViewModel
        {
            PersonalReport = personalReport,
            CurrentChallenge = currentChallenge,
            CurrentGoal = currentGoal,
            LinkedValue = linkedValue,
            GoalRationale = goalRationale
        };

        if (!personalReport.TeamId.HasValue)
        {
            pageModel.ErrorMessage = "Join a team to generate coaching prompts.";
            return View(pageModel);
        }

        TeamReportViewModel? teamReport = null;

        var teamResponse = await client.GetAsync($"/api/reports/team/{personalReport.TeamId.Value}");
        if (teamResponse.IsSuccessStatusCode)
        {
            var teamJson = await teamResponse.Content.ReadAsStringAsync();

            teamReport = JsonSerializer.Deserialize<TeamReportViewModel>(teamJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (teamReport != null)
                teamReport = NormalizeTeamReport(teamReport);
        }

        var coachingRequest = new CoachingRequestViewModel
        {
            UserId = personalReport.UserId,
            TeamId = personalReport.TeamId.Value,
            AlignmentScore = personalReport.CulturalFitScore ?? personalReport.AlignmentWithTeamTop5 ?? 0,
            AlignmentWithTeamTop5 = personalReport.AlignmentWithTeamTop5,
            TeamCultureType = personalReport.TeamCultureType,
            CurrentChallenge = currentChallenge,
            CurrentGoal = currentGoal,
            LinkedValue = linkedValue,
            GoalRationale = goalRationale,

            DominantValues = personalReport.TopValues
                .OrderBy(x => x.Rank)
                .Take(3)
                .Select(x => x.Name)
                .ToList(),

            TeamTopValues = teamReport?.TopValues
                .OrderBy(x => x.Rank)
                .Take(5)
                .Select(x => x.Name)
                .ToList() ?? new List<string>(),

            TeamLowestValues = teamReport?.LowestValues
                .Select(x => x.Name)
                .ToList() ?? new List<string>(),

            TeamTensionFields = teamReport?.TensionFields ?? new List<string>(),

            ReflectionInsights = personalReport.ReflectionInsights
                .SelectMany(x => new[] { x.Meaning, x.Behavior, x.Friction })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList()
        };

        var endpoint = mode.Equals("quick", StringComparison.OrdinalIgnoreCase)
            ? "/api/AiCoach/fallback"
            : "/api/AiCoach/generate";

        var coachingJson = JsonSerializer.Serialize(coachingRequest);

        var coachingResponse = await client.PostAsync(
            endpoint,
            new StringContent(coachingJson, Encoding.UTF8, "application/json"));

        if (coachingResponse.IsSuccessStatusCode)
        {
            var coachingContent = await coachingResponse.Content.ReadAsStringAsync();

            pageModel.CoachingResult =
                JsonSerializer.Deserialize<CoachingResponseViewModel>(coachingContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (pageModel.CoachingResult != null)
                pageModel.CoachingResult = NormalizeCoachingResponse(pageModel.CoachingResult);
        }
        else
        {
            pageModel.ErrorMessage = mode.Equals("quick", StringComparison.OrdinalIgnoreCase)
                ? "Quick coaching could not be generated right now."
                : "AI coaching prompts could not be generated right now. You can still use the suggested reflection prompts below.";
        }

        return View(pageModel);
    }

    [HttpGet]
    public async Task<IActionResult> Team(int teamId)
    {
        var client = CreateAuthorizedClient();

        var response = await client.GetAsync($"/api/reports/team/{teamId}");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Personal));

        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<TeamReportViewModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new TeamReportViewModel();

        return View(NormalizeTeamReport(model));
    }

    [HttpGet]
    public async Task<IActionResult> TeamCoaching(int teamId, string mode = "ai")
    {
        var client = CreateAuthorizedClient();

        var teamResponse = await client.GetAsync($"/api/reports/team/{teamId}");
        if (!teamResponse.IsSuccessStatusCode)
            return RedirectToAction(nameof(Personal));

        var teamJson = await teamResponse.Content.ReadAsStringAsync();

        var teamReport = JsonSerializer.Deserialize<TeamReportViewModel>(teamJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new TeamReportViewModel();

        teamReport = NormalizeTeamReport(teamReport);

        var pageModel = new TeamCoachingPageViewModel
        {
            TeamReport = teamReport
        };

        var coachingRequest = new TeamCoachingRequestViewModel
        {
            TeamId = teamReport.TeamId,
            TeamName = teamReport.TeamName,
            CultureType = teamReport.CultureType,
            AlignmentScore = teamReport.AlignmentScore,
            PolarizationScore = teamReport.PolarizationScore,
            MaturityIndex = teamReport.MaturityIndex,
            TeamSize = teamReport.TeamSize,
            CompletedMembers = teamReport.CompletedMembers,
            TotalMembers = teamReport.TotalMembers,

            TopValues = teamReport.TopValues
                .OrderBy(x => x.Rank)
                .Take(5)
                .Select(x => x.Name)
                .ToList(),

            LowestValues = teamReport.LowestValues
                .Select(x => x.Name)
                .ToList(),

            SharedCoreValues = teamReport.SharedCoreValues,
            TensionFields = teamReport.TensionFields
        };

        var endpoint = mode.Equals("quick", StringComparison.OrdinalIgnoreCase)
            ? "/api/AiCoach/fallback-team"
            : "/api/AiCoach/generate-team";

        var coachingJson = JsonSerializer.Serialize(coachingRequest);

        var coachingResponse = await client.PostAsync(
            endpoint,
            new StringContent(coachingJson, Encoding.UTF8, "application/json"));

        if (coachingResponse.IsSuccessStatusCode)
        {
            var coachingContent = await coachingResponse.Content.ReadAsStringAsync();

            pageModel.CoachingResult =
                JsonSerializer.Deserialize<TeamCoachingResponseViewModel>(coachingContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (pageModel.CoachingResult != null)
                pageModel.CoachingResult = NormalizeTeamCoachingResponse(pageModel.CoachingResult);
        }
        else
        {
            pageModel.ErrorMessage = mode.Equals("quick", StringComparison.OrdinalIgnoreCase)
                ? "Quick team coaching could not be generated right now."
                : "Team AI coaching could not be generated right now. Review the team report data and try again.";
        }

        return View(pageModel);
    }
}
