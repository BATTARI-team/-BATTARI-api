using BATTARI_api.Models;
using BATTARI_api.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace BATTARI_api.Data;

public class UserContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<FriendModel> Friends { get; set; }
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    public DbSet<CallModel> Calls { get; set; }

    protected override void
    OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=user.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().ToTable("Users");
    }
}
