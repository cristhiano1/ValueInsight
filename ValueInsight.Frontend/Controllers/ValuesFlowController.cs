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
        public async Task<IActionResult> ReduceToFive(ValueSelectionViewModel model)
        {
            var raw = HttpContext.Session.GetString("SelectedTop10");
            if (string.IsNullOrWhiteSpace(raw))
                return RedirectToAction(nameof(SelectTen));

            var selectedTop10Ids = JsonSerializer.Deserialize<List<int>>(raw) ?? new();
            model.AvailableValues = await LoadTopFiveValues(selectedTop10Ids);

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

            HttpContext.Session.SetString("SelectedTop5", JsonSerializer.Serialize(orderedTop5));

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

            return RedirectToAction(nameof(ConcretizeTopThree));
        }



        [HttpGet]
        public async Task<IActionResult> ConcretizeTopThree()
        {
            var raw = HttpContext.Session.GetString("SelectedTop5");
            if (string.IsNullOrWhiteSpace(raw))
                return RedirectToAction(nameof(SelectTen));

            var top5 = JsonSerializer.Deserialize<List<int>>(raw) ?? new();
            var topValues = await LoadTopFiveValues(top5);
            var orderedTop3 = topValues.OrderBy(v => top5.IndexOf(v.Id)).Take(3).ToList();

            return View(new ConcretizeTopValuesViewModel
            {
                TopValues = orderedTop3.Select(v => new TopValueReflectionInputViewModel
                {
                    ValueId = v.Id,
                    ValueName = v.Name
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> ConcretizeTopThree(ConcretizeTopValuesViewModel model)
        {
            if (model.TopValues.Count != 3)
            {
                ModelState.AddModelError("", "Please complete all three top values.");
                return View(model);
            }

            foreach (var item in model.TopValues)
            {
                if (string.IsNullOrWhiteSpace(item.Meaning) || string.IsNullOrWhiteSpace(item.Behavior) || string.IsNullOrWhiteSpace(item.Friction))
                {
                    ModelState.AddModelError("", "Please answer all reflection questions for each value.");
                    return View(model);
                }
            }

            var payload = new SaveReflectionRequest
            {
                Answers = model.TopValues.SelectMany(item => new[]
                {
                    new ReflectionAnswerViewModel { QuestionId = $"topvalue-{item.ValueId}-meaning", QuestionText = $"What does {item.ValueName} mean to you?", ResponseText = item.Meaning },
                    new ReflectionAnswerViewModel { QuestionId = $"topvalue-{item.ValueId}-behavior", QuestionText = $"How does {item.ValueName} show up in your behaviour?", ResponseText = item.Behavior },
                    new ReflectionAnswerViewModel { QuestionId = $"topvalue-{item.ValueId}-friction", QuestionText = $"What happens when {item.ValueName} is not respected?", ResponseText = item.Friction },
                }).ToList()
            };

            var client = CreateAuthorizedClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/reflection/save", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Could not save your value reflections.");
                return View(model);
            }

            return RedirectToAction("Personal", "Reports");
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