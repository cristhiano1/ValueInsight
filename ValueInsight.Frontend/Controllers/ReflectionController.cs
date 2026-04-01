using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ValueInsight.Frontend.Models;

namespace ValueInsight.Frontend.Controllers
{
    public class ReflectionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ReflectionController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private HttpClient CreateAuthorizedClient()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiSettings:BaseUrl"];

            client.BaseAddress = new Uri(baseUrl!);

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        [HttpGet]
        public async Task<IActionResult> Mine()
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("/api/reflection/mine");

            if (!response.IsSuccessStatusCode)
                return Unauthorized();

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveReflectionRequest request)
        {
            var client = CreateAuthorizedClient();

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("/api/reflection/save", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
    }
}