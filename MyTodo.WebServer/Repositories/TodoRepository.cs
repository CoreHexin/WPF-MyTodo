using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyTodo.WebServer.Data;
using MyTodo.WebServer.DTOs.Todo;
using MyTodo.WebServer.Helpers;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public TodoRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Todo?> CreateAsync(TodoForCreateDTO todoForCreateDTO)
        {
            Todo todo = _mapper.Map<Todo>(todoForCreateDTO);
            var now = DateTime.Now;
            todo.CreatedAt = now;
            todo.UpdatedAt = now;
            await _dbContext.Todos.AddAsync(todo);

            int result = await _dbContext.SaveChangesAsync();
            if (result == 0)
                return null;
            return todo;
        }

        public async Task<Todo?> DeleteAsync(int id)
        {
            var todo = await _dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null)
            {
                return null;
            }

            _dbContext.Todos.Remove(todo);
            await _dbContext.SaveChangesAsync();
            return todo;
        }

        public async Task<List<Todo>> GetAllAsync(TodoQueryObject queryObject)
        {
            IQueryable<Todo> todoQueryable = _dbContext.Todos.AsQueryable();

            if (queryObject.Status != -1)
            {
                todoQueryable = todoQueryable.Where(t => t.Status == queryObject.Status);
            }

            if (!string.IsNullOrWhiteSpace(queryObject.Title))
            {
                todoQueryable = todoQueryable.Where(t => t.Title.Contains(queryObject.Title));
            }

            List<Todo> todos = await todoQueryable
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            return todos;
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            return await _dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Todo?> UpdateAsync(int id, TodoForUpdateDTO todoForUpdateDTO)
        {
            Todo? todo = await _dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null)
            {
                return null;
            }

            todo.Title = todoForUpdateDTO.Title;
            todo.Content = todoForUpdateDTO.Content;
            todo.Status = todoForUpdateDTO.Status;
            todo.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            return todo;
        }

        public async Task<StatisticDTO> GetStatisticAsync()
        {
            int totalCount = await _dbContext.Todos.CountAsync();
            int finishedCount = await _dbContext.Todos.Where(t => t.Status == 1).CountAsync();
            return new StatisticDTO() { TotalCount = totalCount, FinishedCount = finishedCount };
        }
    }
}
