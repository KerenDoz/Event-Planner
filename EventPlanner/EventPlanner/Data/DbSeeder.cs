using EventPlanner.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "Meetup" },
                new Category { Name = "Workshop" },
                new Category { Name = "Concert" },
                new Category { Name = "Sport" }
            );
        }

        if (!await db.Locations.AnyAsync())
        {
            db.Locations.AddRange(
                new Location { Name = "Tech Hub", City = "Sofia", Address = "Main street 1" },
                new Location { Name = "City Hall", City = "Plovdiv", Address = "Center" }
            );
        }

        await db.SaveChangesAsync();
    }
}
