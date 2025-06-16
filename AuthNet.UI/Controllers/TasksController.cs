using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Failed to create task.");
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
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Failed to update product.");

            return View(model);
        }

        [HttpPost("Delete/Task/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Tasks/{id}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Index", "Home");
        }

    }
}
