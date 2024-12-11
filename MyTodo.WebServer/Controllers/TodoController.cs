using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTodo.WebServer.Data;
using MyTodo.WebServer.DTOs.Todo;

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
    }
}
