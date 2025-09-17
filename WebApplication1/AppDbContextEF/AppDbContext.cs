using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.AppDbContextEF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
    }
}
