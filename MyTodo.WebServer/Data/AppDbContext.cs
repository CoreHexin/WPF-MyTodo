using Microsoft.EntityFrameworkCore;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions options) : base(options) { }


    }
}
