using Microsoft.AspNetCore.Mvc;
using MyTodo.WebServer.DTOs.Todo;
using MyTodo.WebServer.Models;
using MyTodo.WebServer.Repositories;

namespace MyTodo.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        [HttpGet("statistic")]
        public async Task<IActionResult> GetStatistic()
        {
            var response = new ApiResponse();
            StatisticDTO statisticDTO = await _todoRepository.GetStatisticAsync();
            response.IsSuccess = true;
            response.Data = statisticDTO;
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoForCreateDTO todoForCreateDTO)
        {
            var response = new ApiResponse();
            Todo? todo = await _todoRepository.CreateAsync(todoForCreateDTO);
            if (todo == null)
            {
                response.IsSuccess = false;
                response.Message = "创建数据失败";
                return BadRequest(response);
            }
            response.IsSuccess = true;
            response.Data = todo;
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ApiResponse();
            List<Todo> todos = await _todoRepository.GetAllAsync();
            response.IsSuccess = true;
            response.Data = todos;
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]TodoForUpdateDTO todoForUpdateDTO)
        {
            var response = new ApiResponse();
            Todo? todo = await _todoRepository.UpdateAsync(id, todoForUpdateDTO);
            if (todo == null)
            {
                return NotFound();
            }
            response.IsSuccess = true;
            response.Data = todo;
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var response = new ApiResponse();
            Todo? todo = await _todoRepository.DeleteAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            response.IsSuccess = true;
            return Ok(response);
        }
    }
}
