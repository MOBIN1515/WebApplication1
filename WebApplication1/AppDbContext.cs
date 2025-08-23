using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

//"Server=DESKTOP-NIS2M55\\MOBINSQL2025;Database=Mobin;Trusted_Connection=True;TrustServerCertificate=True;"
namespace WebApplication1;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 👇 اینجا مشخص می‌کنیم جدول دقیقاً به اسم users هست
        modelBuilder.Entity<User>().ToTable("users");
    }
}
