using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ValueInsight.Frontend.Models;

namespace ValueInsight.Frontend.Controllers;

public class TeamsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public TeamsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
    public async Task<IActionResult> Workspace()
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/teams/workspace");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Index", "Dashboard");

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var model = new TeamWorkspaceViewModel
        {
            IsAdmin = root.GetProperty("isAdmin").GetBoolean()
        };

        if (root.TryGetProperty("currentTeamId", out var currentTeamIdEl) && currentTeamIdEl.ValueKind != JsonValueKind.Null)
            model.CurrentTeamId = currentTeamIdEl.GetInt32();
        if (root.TryGetProperty("joinRequestStatus", out var statusEl) && statusEl.ValueKind != JsonValueKind.Null)
            model.JoinRequestStatus = statusEl.GetString();
        if (root.TryGetProperty("joinRequestTeamName", out var teamNameEl) && teamNameEl.ValueKind != JsonValueKind.Null)
            model.JoinRequestTeamName = teamNameEl.GetString();
        if (root.TryGetProperty("joinRequestedAtUtc", out var requestedAtEl) && requestedAtEl.ValueKind != JsonValueKind.Null)
            model.JoinRequestedAtUtc = requestedAtEl.GetDateTime();

        if (root.TryGetProperty("manageableTeams", out var teamsEl))
        {
            foreach (var item in teamsEl.EnumerateArray())
            {
                model.ManageableTeams.Add(new ManageableTeamCardViewModel
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Name = item.GetProperty("name").GetString() ?? string.Empty,
                    InviteCode = item.GetProperty("inviteCode").GetString() ?? string.Empty,
                    LeaderName = item.TryGetProperty("leaderName", out var leaderEl) && leaderEl.ValueKind != JsonValueKind.Null ? leaderEl.GetString() : null,
                    MembersCount = item.GetProperty("membersCount").GetInt32(),
                    CompletedMembers = item.GetProperty("completedMembers").GetInt32(),
                    PendingJoinRequests = item.GetProperty("pendingJoinRequests").GetInt32(),
                    AllowPartialReport = item.GetProperty("allowPartialReport").GetBoolean(),
                    TeamReportReady = item.GetProperty("teamReportReady").GetBoolean()
                });
            }
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Join(string? message = null)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/teams");
        var teams = new List<TeamOptionViewModel>();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            teams = JsonSerializer.Deserialize<List<TeamOptionViewModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<TeamOptionViewModel>();
        }

        return View(new TeamJoinPageViewModel
        {
            Teams = teams,
            Message = message
        });
    }

    [HttpPost]
    public async Task<IActionResult> Join(TeamJoinPageViewModel model)
    {
        var client = CreateAuthorizedClient();
        var payload = new
        {
            inviteCode = model.InviteCode
        };

        var response = await client.PostAsync("/api/teams/join-request",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            model.Message = $"Could not send join request: {error}";
            return await Join(model.Message);
        }

        return RedirectToAction(nameof(Workspace));
    }

    [HttpGet]
    public async Task<IActionResult> Manage(int teamId, string? message = null)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"/api/teams/{teamId}");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Workspace));

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var model = new TeamManagementViewModel
        {
            TeamId = root.GetProperty("id").GetInt32(),
            TeamName = root.GetProperty("name").GetString() ?? string.Empty,
            InviteCode = root.GetProperty("inviteCode").GetString() ?? string.Empty,
            AllowPartialReport = root.GetProperty("allowPartialReport").GetBoolean(),
            Message = message
        };

        if (root.TryGetProperty("leaderId", out var leaderIdEl) && leaderIdEl.ValueKind != JsonValueKind.Null)
            model.LeaderId = leaderIdEl.GetInt32();
        if (root.TryGetProperty("leaderName", out var leaderNameEl))
            model.LeaderName = leaderNameEl.GetString();
        if (root.TryGetProperty("completedMembers", out var completedEl))
            model.CompletedMembers = completedEl.GetInt32();
        if (root.TryGetProperty("totalMembers", out var totalEl))
            model.TotalMembers = totalEl.GetInt32();
        if (root.TryGetProperty("teamReportReady", out var readyEl))
            model.TeamReportReady = readyEl.GetBoolean();
        if (root.TryGetProperty("canManage", out var canManageEl))
            model.CanManage = canManageEl.GetBoolean();

        if (root.TryGetProperty("members", out var membersEl))
        {
            foreach (var item in membersEl.EnumerateArray())
            {
                model.Members.Add(new TeamManagementMemberViewModel
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Name = item.GetProperty("name").GetString() ?? string.Empty,
                    Email = item.GetProperty("email").GetString() ?? string.Empty,
                    IsLeader = item.GetProperty("isLeader").GetBoolean(),
                    HasCompletedAssessment = item.GetProperty("hasCompletedAssessment").GetBoolean()
                });
            }
        }

        if (root.TryGetProperty("pendingJoinRequests", out var pendingEl))
        {
            foreach (var item in pendingEl.EnumerateArray())
            {
                model.PendingJoinRequests.Add(new PendingJoinRequestViewModel
                {
                    Id = item.GetProperty("id").GetInt32(),
                    UserId = item.GetProperty("userId").GetInt32(),
                    UserName = item.GetProperty("userName").GetString() ?? string.Empty,
                    UserEmail = item.GetProperty("userEmail").GetString() ?? string.Empty,
                    Status = item.GetProperty("status").GetString() ?? string.Empty,
                    RequestedAtUtc = item.GetProperty("requestedAtUtc").GetDateTime()
                });
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AssignLeader(int teamId, int userId)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsync($"/api/teams/{teamId}/assign-leader",
            new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json"));

        var message = response.IsSuccessStatusCode ? "Team leader updated." : $"Could not update team leader: {await response.Content.ReadAsStringAsync()}";
        return RedirectToAction(nameof(Manage), new { teamId, message });
    }

    [HttpPost]
    public async Task<IActionResult> SetPartialReport(int teamId, bool allowPartialReport)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsync($"/api/teams/{teamId}/allow-partial-report",
            new StringContent(JsonSerializer.Serialize(new { allowPartialReport }), Encoding.UTF8, "application/json"));

        var message = response.IsSuccessStatusCode ? "Team report policy updated." : $"Could not update report policy: {await response.Content.ReadAsStringAsync()}";
        return RedirectToAction(nameof(Manage), new { teamId, message });
    }

    [HttpPost]
    public async Task<IActionResult> ReviewJoinRequest(int teamId, int requestId, bool approve)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsync($"/api/teams/join-request/{requestId}/review",
            new StringContent(JsonSerializer.Serialize(new { approve }), Encoding.UTF8, "application/json"));

        var message = response.IsSuccessStatusCode ? (approve ? "Join request approved." : "Join request rejected.") : $"Could not review join request: {await response.Content.ReadAsStringAsync()}";
        return RedirectToAction(nameof(Manage), new { teamId, message });
    }
}
