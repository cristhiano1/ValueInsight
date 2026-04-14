using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ValueInsight.Frontend.Models;

namespace ValueInsight.Frontend.Controllers;

public class DashboardController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public DashboardController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");
        var teamId = HttpContext.Session.GetString("TeamId");

        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        var client = CreateAuthorizedClient();
        role ??= "User";

        Console.WriteLine("ROLE: " + role);
        Console.WriteLine("TEAMID: " + teamId);

        if (role.ToLower() == "coach")
        {
            var userName = HttpContext.Session.GetString("Name") ?? "Coach";

            if (!int.TryParse(teamId, out var parsedTeamId))
            {
                return View("Coach", new DashboardViewModel
                {
                    UserName = userName,
                    TeamCultureType = "No data",
                    TeamName = "No team"
                });
            }

            try
            {
                var teamResponse = await client.GetAsync($"/api/teams/{parsedTeamId}/overview");

                if (!teamResponse.IsSuccessStatusCode)
                {
                    return View("Coach", new DashboardViewModel
                    {
                        UserName = userName,
                        TeamId = parsedTeamId,
                        TeamCultureType = "No data"
                    });
                }

                var teamJson = await teamResponse.Content.ReadAsStringAsync();

                var teamData = JsonSerializer.Deserialize<DashboardViewModel>(
                    teamJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                ) ?? new DashboardViewModel();

                teamData.UserName = userName;

                if (teamData.TeamId is null)
                    teamData.TeamId = parsedTeamId;

                if (string.IsNullOrWhiteSpace(teamData.TeamCultureType))
                    teamData.TeamCultureType = "No data";

                return View("Coach", teamData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("COACH DASHBOARD ERROR: " + ex);

                return View("Coach", new DashboardViewModel
                {
                    UserName = userName,
                    TeamId = parsedTeamId,
                    TeamCultureType = "No data"
                });
            }
        }

        var response = await client.GetAsync("/api/reports/dashboard");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Personal", "Reports");
        }

        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<DashboardViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new DashboardViewModel();

        return View("User", model);
    }
}