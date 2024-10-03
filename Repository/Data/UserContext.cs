using BATTARI_api.Models;
using Microsoft.EntityFrameworkCore;

namespace BATTARI_api.Data;

public class UserContext : DbContext
{
  public UserContext() : base() { }

  public DbSet<UserModel> Users { get; set; }
  public DbSet<FriendModel> Friends { get; set; }
  public DbSet<RefreshTokenModel> RefreshTokens { get; set; }

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
