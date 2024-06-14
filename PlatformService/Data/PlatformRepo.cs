using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepo : IPlatformRepo
{
    private readonly AppDbContext dbContext;

    public PlatformRepo(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void CreatePlatform(Platform platform)
    {
        if(platform is null){
            throw new ArgumentNullException(nameof(platform));
        }

        dbContext.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return dbContext.Platforms.ToList();
    }

    public Platform GetPlatformById(int id)
    {
        return dbContext.Platforms.FirstOrDefault(p=> p.Id==id);
    }

    public bool SaveChanges()
    {
       return (dbContext.SaveChanges() >= 0 );
    }
}
