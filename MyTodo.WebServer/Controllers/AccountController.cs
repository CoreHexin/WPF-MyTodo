using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTodo.WebServer.Data;
using MyTodo.WebServer.DTOs;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AccountController(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _dbContext
                .Users.Where(u => u.Email == loginDTO.Email)
                .Where(u => u.Password == loginDTO.Password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            var newUserDTO = new NewUserDTO { Email = user.Email, Name = user.Name };
            return Ok(newUserDTO);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var user = await _dbContext
                .Users.Where(u => u.Email == registerDTO.Email)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                return BadRequest("账号已被注册");
            }

            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                return BadRequest("密码不一致");
            }

            var newUser = new User
            {
                Email = registerDTO.Email,
                Name = registerDTO.Name,
                Password = registerDTO.Password,
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            var newUserDTO = new NewUserDTO { Email = registerDTO.Email, Name = registerDTO.Name };
            return Ok(newUserDTO);
        }
    }
}
