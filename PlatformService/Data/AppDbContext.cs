using Microsoft.EntityFrameworkCore;
using PlatformService.Model;

namespace PlatformService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opt): base(opt)
        {
        }

        public DbSet<Platform> Platforms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            base.OnConfiguring(optionsBuilder);
        }
    }
}
