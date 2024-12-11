using System.ComponentModel.DataAnnotations;

namespace MyTodo.WebServer.DTOs.Account
{
    public class NewUserDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
