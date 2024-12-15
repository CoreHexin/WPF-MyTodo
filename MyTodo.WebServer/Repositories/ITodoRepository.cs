using MyTodo.WebServer.DTOs.Todo;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Repositories
{
    public interface ITodoRepository
    {
        Task<List<Todo>> GetAllAsync();
        Task<Todo?> GetByIdAsync(int id);
        Task<Todo?> CreateAsync(TodoForCreateDTO todoForCreateDTO);
        Task<Todo?> UpdateAsync(int id, TodoForUpdateDTO todoForUpdateDTO);
        Task<Todo?> DeleteAsync(int id);
        Task<StatisticDTO> GetStatisticAsync();
    }
}
