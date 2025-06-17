using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ITaskService
    {
        List<TaskItemDto> GetAllTasks();
        TaskItem? GetTaskById(int id);
        Task<int> GetTaskCountAsync();
        OperationResponse AddTask(TaskItemDto dto);
        OperationResponse UpdateTask(int id, TaskItemDto dto);
        OperationResponse DeleteTask(int id);
    }
}
