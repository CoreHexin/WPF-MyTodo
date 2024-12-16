using MyTodo.WebServer.DTOs.Todo;
using MyTodo.WebServer.Helpers;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Repositories
{
    public interface ITodoRepository
    {
        Task<List<Todo>> GetAllAsync(TodoQueryObject queryObject);
        Task<Todo?> GetByIdAsync(int id);
        Task<Todo?> CreateAsync(TodoForCreateDTO todoForCreateDTO);
        Task<Todo?> UpdateAsync(int id, TodoForUpdateDTO todoForUpdateDTO);
        Task<Todo?> DeleteAsync(int id);
        Task<StatisticDTO> GetStatisticAsync();
    }
}
