using Microsoft.EntityFrameworkCore;
using PlatformService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }
        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            _context.Add(platform);
        }

        public async Task<IEnumerable<Platform>> GetAllPlatforms()
        { 
            return await _context.Platforms.ToListAsync();
        }

        public async Task<Platform> GetPlaformById(int id)
        {
            return await _context.Platforms.FirstOrDefaultAsync(p=> p.Id == id);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
