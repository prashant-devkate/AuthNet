using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;

namespace AuthNet.UI.Controllers
{
    public class TasksController : Controller
    {
        private readonly HttpClient _httpClient;

        public TasksController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var tasksResponse = await _httpClient.GetAsync("/api/Tasks");

            var tasksList = new List<TaskItemDto>();

            if (tasksResponse.IsSuccessStatusCode)
            {
                var tasksContent = await tasksResponse.Content.ReadAsStringAsync();
                var tasks = JsonConvert.DeserializeObject<List<TaskItemDto>>(tasksContent);

                tasksList = tasks.Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    BadgeColor = GetBadgeColor(t.DueDate),
                    DueLabel = GetDueLabel(t.DueDate)
                }).ToList();
            }

            return View(tasksList);
        }

        private string GetBadgeColor(DateTime dueDate)
        {
            if (dueDate.Date < DateTime.Today)
                return "danger"; // Red
            else if (dueDate.Date == DateTime.Today)
                return "warning"; // Orange
            else
                return "success"; // Green
        }

        private string GetDueLabel(DateTime dueDate)
        {
            if (dueDate.Date < DateTime.Today)
                return "Overdue";
            else if (dueDate.Date == DateTime.Today)
                return "Due Today";
            else
                return "Upcoming";
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddTaskItemViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTaskItemViewModel task)
        {
            if (!ModelState.IsValid)
                return View(task);

            var response = await _httpClient.PostAsJsonAsync("api/Tasks", task);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Task added successfully.";
                return RedirectToAction("Index", "Tasks");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to add task.";

            ModelState.AddModelError("", errorMsg);
            return View(task);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _httpClient.GetFromJsonAsync<TaskItemDto>($"api/Tasks/{id}");
            if (task == null) return NotFound();

            var model = new EditTaskItemViewModel
            {
                TaskId = task.Id,
                Title = task.Title,
                DueDate = task.DueDate
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTaskItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var response = await _httpClient.PutAsJsonAsync($"api/Tasks/{model.TaskId}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Task updated successfully.";
                return RedirectToAction("Index", "Tasks");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update task.";

            ModelState.AddModelError("", errorMsg);
            return View(model);
        }

        [HttpPost("Delete/Task/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Tasks/{id}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to delete task.";

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Index", "Tasks");
            }

            TempData["SuccessMessage"] = "Task deleted successfully.";
            return RedirectToAction("Index", "Tasks");

        }

    }
}
