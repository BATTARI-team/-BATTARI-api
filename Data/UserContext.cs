using BATTARI_api.Models;
using Microsoft.EntityFrameworkCore;

namespace BATTARI_api.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }
    
    public DbSet<UserModel> Users { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=user.db");
    }
}