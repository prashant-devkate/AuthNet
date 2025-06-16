using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public List<TaskItemDto> GetAllTasks()
        {
            var today = DateTime.Today;

            var taskEntities = _context.Tasks
                .Select(t => new { t.Id, t.Title, t.DueDate }) 
                .ToList();

            var taskDtos = taskEntities.Select(task => new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                DueDate = task.DueDate,
                DueLabel = GetDueLabel(task.DueDate, today),
                BadgeColor = GetBadgeColor(task.DueDate, today)
            }).ToList();

            return taskDtos;
        }

        public TaskItem? GetTaskById(int id)
        {
            return _context.Tasks.Find(id);
        }

        public async Task<int> GetTaskCountAsync()
        {
            return await _context.Tasks.CountAsync();
        }

        public bool AddTask(TaskItemDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                DueDate = dto.DueDate
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateTask(int id, TaskItemDto dto)
        {
            var task = _context.Tasks.Find(id);
            if (task == null) return false;

            task.Title = dto.Title;
            task.DueDate = dto.DueDate;

            _context.SaveChanges();
            return true;
        }

        public bool DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return true;
        }

        private string GetDueLabel(DateTime dueDate, DateTime today)
        {
            if (dueDate.Date == today) return "Today";
            if (dueDate.Date == today.AddDays(1)) return "Tomorrow";
            if (dueDate.Date <= today.AddDays(7)) return "This Week";
            return dueDate.ToShortDateString();
        }

        private string GetBadgeColor(DateTime dueDate, DateTime today)
        {
            if (dueDate.Date == today) return "danger";
            if (dueDate.Date == today.AddDays(1)) return "warning text-dark";
            if (dueDate.Date <= today.AddDays(7)) return "success";
            return "secondary";
        }
    }

}
