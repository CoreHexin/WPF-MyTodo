using Microsoft.AspNetCore.Mvc;
using MyTodo.WebServer.DTOs.Memo;
using MyTodo.WebServer.Models;
using MyTodo.WebServer.Repositories;

namespace MyTodo.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemoController : ControllerBase
    {
        private readonly IMemoRepository _memoRepository;

        public MemoController(IMemoRepository memoRepository)
        {
            _memoRepository = memoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? title)
        {
            List<Memo> memos = await _memoRepository.GetAllAsync(title);

            ApiResponse response = new ApiResponse();
            response.IsSuccess = true;
            response.Data = memos;
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MemoForCreateDTO memoDTO)
        {
            Memo? memo = await _memoRepository.CreateAsync(memoDTO);

            ApiResponse response = new ApiResponse();
            if (memo == null)
            {
                return BadRequest();
            }

            response.IsSuccess = true;
            response.Data = memo;
            return CreatedAtAction(nameof(GetById), new { id = memo.Id }, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            Memo? memo = await _memoRepository.GetByIdAsync(id);

            ApiResponse response = new ApiResponse();
            if (memo == null)
            {
                return NotFound();
            }

            response.IsSuccess = true;
            response.Data = memo;
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            Memo? memo = await _memoRepository.DeleteAsync(id);
            if (memo == null)
            {
                return NotFound();
            }

            ApiResponse response = new ApiResponse();
            response.IsSuccess = true;
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] MemoForUpdateDTO memoDTO)
        {
            Memo? memo = await _memoRepository.UpdateAsync(id, memoDTO);
            if (memo == null)
            {
                return NotFound();
            }

            ApiResponse response = new ApiResponse();
            response.IsSuccess = true;
            response.Data = memo;
            return Ok(response);
        }

        [HttpGet("total")]
        public async Task<IActionResult> Count()
        {
            int count = await _memoRepository.CountAsync();
            ApiResponse response = new ApiResponse();
            response.IsSuccess = true;
            response.Data = count;
            return Ok(response);
        }
    }
}
