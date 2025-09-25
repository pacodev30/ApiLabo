using ApiLabo.Data.Models;
using Microsoft.EntityFrameworkCore;
namespace ApiLabo.Data;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(c =>
        {
            c.ToTable("User");
            c.Property(c => c.Pseudo).HasMaxLength(50);
            c.Property(c => c.Password).HasMaxLength(255);
        });
        base.OnModelCreating(modelBuilder);
    }
}
