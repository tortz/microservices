
using PlatformService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        Task<bool> SaveChanges();
        Task<IEnumerable<Platform>> GetAllPlatforms();
        Task<Platform> GetPlaformById(int id);
        void CreatePlatform(Platform platform);
    }
}
