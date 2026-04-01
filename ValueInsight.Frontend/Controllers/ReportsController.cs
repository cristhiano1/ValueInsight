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

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> CoachingPrompts()
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

        return View(model);
    }
}
