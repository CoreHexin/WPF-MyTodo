using AutoMapper;
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
        public async Task<IActionResult> SaveTodoAsync(TodoDTO todoDTO)
        {
            var response = new ApiResponse();

            var now = DateTime.Now;
            Todo newTodo = _mapper.Map<Todo>(todoDTO);
            newTodo.CreatedAt = now;
            newTodo.UpdatedAt = now;

            _appDbContext.Todos.Add(newTodo);

            try
            {
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "服务器异常，请稍后重试";
                return BadRequest(response);
            }

            response.IsSuccess = true;
            response.Data = newTodo;
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTodosAsync()
        {
            var response = new ApiResponse();
            List<Todo> todoItems;

            try
            {
                todoItems = await _appDbContext
                    .Todos.OrderBy(t => t.Status)
                    .ThenByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "服务器异常，请稍后重试";
                return BadRequest(response);
            }

            response.IsSuccess = true;
            response.Data = todoItems;
            return Ok(response);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateTodoStatusAsync(
            TodoStatusUpdateDTO todoDTO
        )
        {
            var response = new ApiResponse();

            try
            {
                var todoItem = await _appDbContext.Todos.FindAsync(todoDTO.Id);
                if (todoItem == null)
                {
                    return NotFound();
                }
                todoItem.Status = todoDTO.Status;
                await _appDbContext.SaveChangesAsync();
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "服务器异常，请稍后重试";
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTodo(TodoDTO todoDTO)
        {
            var response = new ApiResponse();

            try
            {
                Todo? todoItem = await _appDbContext.Todos.FindAsync(todoDTO.Id);
                if (todoItem == null)
                {
                    return NotFound();
                }

                todoItem.Title = todoDTO.Title;
                todoItem.Content = todoDTO.Content;
                todoItem.Status = todoDTO.Status;
                todoItem.UpdatedAt = DateTime.Now;
                await _appDbContext.SaveChangesAsync();

                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "服务器异常，请稍后重试";
                return BadRequest(response);
            }
        }
    }
}
