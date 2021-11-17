using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Model;
using System;
using System.Linq;

namespace PlatformService.Data
{
    public static class DbPrep
    {
        public static void PrepPopulation(IApplicationBuilder appBuilder, bool isProd)
        {
            using var serviceScope = appBuilder.ApplicationServices.CreateScope();
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                try
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }
            if (context.Platforms.Any())
            {
                Console.WriteLine("->> We already have data...");
                return;
            }

            Console.WriteLine("->> Seeding data...");
            context.AddRange(
                new Platform() { Name="Dot Net", Publisher="Microsoft", Cost="Free" },
                new Platform() { Name="SQL Service Express", Publisher="Microsoft", Cost="Free" },
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );
            context.SaveChanges();
        }
    }
}
