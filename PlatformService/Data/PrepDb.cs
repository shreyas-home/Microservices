using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb{

    public static void PrepPopulation(IApplicationBuilder application , bool IsProduction)
    {
        using(var serviceScope = application.ApplicationServices.CreateScope())
        {
            seedData(serviceScope.ServiceProvider.GetService<AppDbContext>() , IsProduction);
        }
    }

    private static void seedData(AppDbContext dbContext , bool IsProduction)
    {
        if(IsProduction)
        {
            Console.WriteLine("--> Attempting Migrations");
            try{
                dbContext.Database.Migrate();
            }
            catch(Exception e){
                Console.WriteLine(e.Message);
            }

        }

        if(!dbContext.Platforms.Any()){
            Console.WriteLine("--> Seeding Data ...");

            dbContext.Platforms.AddRange(
                new Platform() {
                    Name="Dot Net",
                    Publisher="Microsoft",
                    Cost="free"
                },
                new Platform() {
                    Name="Sql Server",
                    Publisher="Microsoft",
                    Cost="free"
                },
                new Platform() {
                    Name="Kubernetes",
                    Publisher="Cloud Native Computing",
                    Cost="free"
                }
            );

            dbContext.SaveChanges();
        }
        else{
            Console.WriteLine("--> We already have data");
        }
    }
}