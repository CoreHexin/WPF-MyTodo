using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTodo.WebServer.Data;
using MyTodo.WebServer.DTOs.Account;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AccountController(AppDbContext appDbContext, IMapper mapper)
        {
            _dbContext = appDbContext;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = new ApiResponse();

            var user = await _dbContext
                .Users.Where(u => u.Email == loginDTO.Email)
                .Where(u => u.Password == loginDTO.Password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "账号或密码错误";
                return Ok(response);
            }

            var newUserDTO = _mapper.Map<NewUserDTO>(user);
            response.IsSuccess = true;
            response.Data = newUserDTO;
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var response = new ApiResponse();

            var user = await _dbContext
                .Users.Where(u => u.Email == registerDTO.Email)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                response.IsSuccess = false;
                response.Message = "账号已被注册";
                return Ok(response);
            }

            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                response.IsSuccess = false;
                response.Message = "密码不一致";
                return Ok(response);
            }

            var newUser = new User
            {
                Email = registerDTO.Email,
                Name = registerDTO.Name,
                Password = registerDTO.Password,
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            var newUserDTO = _mapper.Map<NewUserDTO>(registerDTO);
            response.IsSuccess = true;
            response.Data = newUserDTO;
            return Ok(response);
        }
    }
}
