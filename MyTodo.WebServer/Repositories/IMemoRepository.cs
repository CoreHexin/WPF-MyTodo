using MyTodo.WebServer.DTOs.Memo;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Repositories
{
    public interface IMemoRepository
    {
        Task<List<Memo>> GetAllAsync(string? title = null);
        Task<Memo?> GetByIdAsync(int id);
        Task<Memo?> CreateAsync(MemoForCreateDTO MemoForCreateDTO);
        Task<Memo?> UpdateAsync(int id, MemoForUpdateDTO MemoForUpdateDTO);
        Task<Memo?> DeleteAsync(int id);
    }
}
