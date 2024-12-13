using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTodo.WebServer.Data;
using MyTodo.WebServer.DTOs.Todo;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public TodoController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        [HttpGet("statistic")]
        public async Task<IActionResult> GetStatisticAsync()
        {
            var response = new ApiResponse();
            int totalCount;
            int finishedCount;

            try
            {
                totalCount = await _appDbContext.Todos.CountAsync();
                finishedCount = await _appDbContext.Todos.Where(t => t.Status == 1).CountAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "服务器异常，请稍后重试";
                return BadRequest(response);
            }

            response.IsSuccess = true;
            response.Data = new StatisticDTO()
            {
                TotalCount = totalCount,
                FinishedCount = finishedCount,
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTodoItemAsync(TodoItemDTO todoItemDTO)
        {
            var response = new ApiResponse();

            var now = DateTime.Now;
            var newTodo = new Todo()
            {
                Title = todoItemDTO.Title,
                Content = todoItemDTO.Content,
                Status = todoItemDTO.Status,
                CreatedAt = now,
                UpdatedAt = now,
            };
            _appDbContext.Todos.Add(newTodo);
            await _appDbContext.SaveChangesAsync();

            await Task.Delay(2000);
            response.IsSuccess = true;
            response.Data = newTodo;
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTodoItemsAsync()
        {
            var response = new ApiResponse();

            var todoItems = await _appDbContext
                .Todos.OrderBy(t => t.Status)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            response.IsSuccess = true;
            response.Data = todoItems;
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTodoItemStatusAsync(TodoItemStatusUpdateDTO todoItemDTO)
        {
            var response = new ApiResponse();

            var todoItem = await _appDbContext.Todos.FindAsync(todoItemDTO.Id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Status = todoItemDTO.Status;
            await _appDbContext.SaveChangesAsync();

            response.IsSuccess = true;
            return Ok(response);
        }
    }
}
