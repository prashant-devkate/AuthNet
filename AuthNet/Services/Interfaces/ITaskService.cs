using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ITaskService
    {
        List<TaskItemDto> GetAllTasks();
        TaskItem? GetTaskById(int id);
        Task<int> GetTaskCountAsync();
        bool AddTask(TaskItemDto dto);
        bool UpdateTask(int id, TaskItemDto dto);
        bool DeleteTask(int id);
    }
}
