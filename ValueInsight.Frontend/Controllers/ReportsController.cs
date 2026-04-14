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

    private HttpClient CreateAuthorizedClient()
    {
        var token = HttpContext.Session.GetString("JWToken");
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);

        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    // =========================
    // PERSONAL → TODOS LOS USERS
    // =========================
    [HttpGet]
    public async Task<IActionResult> Personal()
    {
        var token = HttpContext.Session.GetString("JWToken");

        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/reports/me");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Login", "Account");

        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<PersonalReportViewModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new PersonalReportViewModel();

        return View(model);
    }

    // =========================
    // COACHING → SOLO COACH
    // =========================
    [HttpGet]
    public async Task<IActionResult> CoachingPrompts()
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        if (role != "Coach")
            return RedirectToAction("Index", "Dashboard");

        var client = CreateAuthorizedClient();
        var reportResponse = await client.GetAsync("/api/reports/me");

        if (!reportResponse.IsSuccessStatusCode)
            return RedirectToAction("Login", "Account");

        var reportJson = await reportResponse.Content.ReadAsStringAsync();
        var personalReport = JsonSerializer.Deserialize<PersonalReportViewModel>(reportJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new PersonalReportViewModel();

        var pageModel = new CoachingPromptsPageViewModel
        {
            PersonalReport = personalReport
        };

        if (!personalReport.TeamId.HasValue)
        {
            pageModel.ErrorMessage = "Join a team to generate AI coaching prompts.";
            return View(pageModel);
        }

        var coachingRequest = new CoachingRequestViewModel
        {
            UserId = personalReport.UserId,
            TeamId = personalReport.TeamId.Value,
            AlignmentScore = personalReport.CulturalFitScore ?? personalReport.AlignmentWithTeamTop5 ?? 0,
            DominantValues = personalReport.TopValues
                .OrderBy(x => x.Rank)
                .Take(3)
                .Select(x => x.Name)
                .ToList()
        };

        var coachingJson = JsonSerializer.Serialize(coachingRequest);

        var coachingResponse = await client.PostAsync(
            "/api/AiCoach/generate",
            new StringContent(coachingJson, Encoding.UTF8, "application/json"));

        if (coachingResponse.IsSuccessStatusCode)
        {
            var coachingContent = await coachingResponse.Content.ReadAsStringAsync();
            pageModel.CoachingResult = JsonSerializer.Deserialize<CoachingResponseViewModel>(coachingContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else
        {
            pageModel.ErrorMessage = "AI coaching prompts could not be generated right now. You can still use the suggested reflection prompts below.";
        }

        return View(pageModel);
    }

    // =========================
    // TEAM → SOLO COACH
    // =========================
    [HttpGet]
    public async Task<IActionResult> Team(int teamId)
    {
        var client = CreateAuthorizedClient();

        var response = await client.GetAsync($"/api/reports/team/{teamId}");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Personal));

        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<TeamReportViewModel>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new TeamReportViewModel();

        // =========================
        // 🤖 NUEVO: LLAMADA AI
        // =========================
        try
        {
            var aiPayload = new
            {
                cultureType = model.CultureType,
                alignmentScore = model.AlignmentScore,
                polarizationScore = model.PolarizationScore,
                maturityIndex = model.MaturityIndex,
                topCategories = model.CategoryProfile
                    .OrderByDescending(x => x.Percentage)
                    .Take(3)
                    .Select(x => x.Category)
                    .ToList(),
                lowCategories = model.CategoryProfile
                    .OrderBy(x => x.Percentage)
                    .Take(2)
                    .Select(x => x.Category)
                    .ToList()
            };

            var aiContent = new StringContent(
                JsonSerializer.Serialize(aiPayload),
                Encoding.UTF8,
                "application/json");

            var aiResponse = await client.PostAsync("/api/AiCoach/team-insight", aiContent);

            if (aiResponse.IsSuccessStatusCode)
            {
                var aiJson = await aiResponse.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(aiJson);
                model.TeamInsight = doc.RootElement
                    .GetProperty("insight")
                    .GetString();
            }
        }
        catch
        {
            model.TeamInsight = null;
        }

        return View(model);
    }
}