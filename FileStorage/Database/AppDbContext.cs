using FileStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }
    }
}
