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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/reports/dashboard");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Personal", "Reports");

        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<DashboardViewModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new DashboardViewModel();

        return View(model);
    }
}
