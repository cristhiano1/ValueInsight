using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ValueInsight.Frontend.Models;

namespace ValueInsight.Frontend.Controllers
{
    public class ValuesFlowController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ValuesFlowController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
        public async Task<IActionResult> SelectTen()
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("/api/values");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Login", "Account");

            var json = await response.Content.ReadAsStringAsync();
            var values = JsonSerializer.Deserialize<List<ValueOptionViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var model = new ValueSelectionViewModel
            {
                Categories = values
                    .GroupBy(v => v.Category)
                    .OrderBy(g => g.Key)
                    .Select(g => new ValueCategoryGroupViewModel
                    {
                        CategoryName = g.Key,
                        Values = g.OrderBy(v => v.Name).ToList()
                    })
                    .ToList()
            };

            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> SelectTen(ValueSelectionViewModel model)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("/api/values");
            var json = await response.Content.ReadAsStringAsync();

            var values = JsonSerializer.Deserialize<List<ValueOptionViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            model.Categories = values
                .GroupBy(v => v.Category)
                .OrderBy(g => g.Key)
                .Select(g => new ValueCategoryGroupViewModel
                {
                    CategoryName = g.Key,
                    Values = g.OrderBy(v => v.Name).ToList()
                })
                .ToList();

            if (model.SelectedValueIds.Count != 10)
            {
                ModelState.AddModelError("", "Please select exactly 10 values.");
                return View(model);
            }

            HttpContext.Session.SetString("SelectedTop10",
                JsonSerializer.Serialize(model.SelectedValueIds));

            return RedirectToAction(nameof(ReduceToFive));
        }

        [HttpGet]
        public async Task<IActionResult> ReduceToFive()
        {
            var raw = HttpContext.Session.GetString("SelectedTop10");
            if (string.IsNullOrWhiteSpace(raw))
                return RedirectToAction(nameof(SelectTen));

            var selectedIds = JsonSerializer.Deserialize<List<int>>(raw) ?? new();

            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("/api/values");
            var json = await response.Content.ReadAsStringAsync();

            var allValues = JsonSerializer.Deserialize<List<ValueOptionViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var model = new ValueSelectionViewModel
            {
                AvailableValues = allValues.Where(v => selectedIds.Contains(v.Id)).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ReduceToFive(ValueSelectionViewModel model)
        {
            if (model.SelectedValueIds.Count != 5)
            {
                ModelState.AddModelError("", "Please reduce your selection to exactly 5 values.");
                return View(model);
            }

            HttpContext.Session.SetString("SelectedTop5",
                JsonSerializer.Serialize(model.SelectedValueIds));

            return RedirectToAction(nameof(RankTopThree));
        }

        [HttpGet]
        public async Task<IActionResult> RankTopThree()
        {
            var raw = HttpContext.Session.GetString("SelectedTop5");
            if (string.IsNullOrWhiteSpace(raw))
                return RedirectToAction(nameof(SelectTen));

            var selectedIds = JsonSerializer.Deserialize<List<int>>(raw) ?? new();

            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("/api/values");
            var json = await response.Content.ReadAsStringAsync();

            var allValues = JsonSerializer.Deserialize<List<ValueOptionViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var model = new RankTopThreeViewModel
            {
                TopFiveValues = allValues.Where(v => selectedIds.Contains(v.Id)).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RankTopThree(RankTopThreeViewModel model)
        {
            var raw = HttpContext.Session.GetString("SelectedTop5");
            if (string.IsNullOrWhiteSpace(raw))
                return RedirectToAction(nameof(SelectTen));

            var top5 = JsonSerializer.Deserialize<List<int>>(raw) ?? new();

            var selectedTop3 = new[] { model.FirstValueId, model.SecondValueId, model.ThirdValueId };

            if (selectedTop3.Distinct().Count() != 3)
            {
                ModelState.AddModelError("", "Top 3 values must be different.");
                model.TopFiveValues = await LoadTopFiveValues(top5);
                return View(model);
            }

            if (selectedTop3.Any(id => !top5.Contains(id)))
            {
                ModelState.AddModelError("", "Top 3 must come from your selected 5.");
                model.TopFiveValues = await LoadTopFiveValues(top5);
                return View(model);
            }

            var orderedTop5 = new List<int>
            {
                model.FirstValueId,
                model.SecondValueId,
                model.ThirdValueId
            };

            orderedTop5.AddRange(top5.Where(id => !orderedTop5.Contains(id)));

            var payload = new
            {
                orderedValueIds = orderedTop5
            };

            var client = CreateAuthorizedClient();
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("/api/user-values/save-top5", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Could not save your ranked values.");
                model.TopFiveValues = await LoadTopFiveValues(top5);
                return View(model);
            }

            // show success message
            ViewBag.SuccessMessage = "Your values were saved successfully!";

            
            model.TopFiveValues = await LoadTopFiveValues(top5);

            return View(model);
        }

        private async Task<List<ValueOptionViewModel>> LoadTopFiveValues(List<int> top5)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("/api/values");
            var json = await response.Content.ReadAsStringAsync();

            var allValues = JsonSerializer.Deserialize<List<ValueOptionViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            return allValues.Where(v => top5.Contains(v.Id)).ToList();
        }
    }
}