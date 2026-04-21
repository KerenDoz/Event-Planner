using EventPlanner.Controllers;
using EventPlanner.Data.Models;
using EventPlanner.Models.Locations;
using EventPlanner.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Tests.Services;

public class LocationsServiceTests
{
    [Fact]
    public async Task Create_ShouldAddLocation_WhenModelIsValid()
    {
        await using var db = TestDbHelper.GetDbContext();
        var controller = new LocationsController(db);
        var model = new LocationFormModel { Name = "Expo Center", City = "Sofia", Address = " Blvd " };

        var result = await controller.Create(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LocationsController.Index), redirect.ActionName);
        Assert.Single(db.Locations);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenLocationMissing()
    {
        await using var db = TestDbHelper.GetDbContext();
        var controller = new LocationsController(db);

        var result = await controller.Edit(999, new LocationFormModel { Name = "A", City = "B" });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldNotDelete_WhenLocationIsUsed()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new LocationsController(db);
        TestDbHelper.SetUser(controller, "organizer-1");
        TestDbHelper.SetTempData(controller);

        var result = await controller.DeleteConfirmed(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LocationsController.Index), redirect.ActionName);
        Assert.NotNull(controller.TempData["Error"]);
        Assert.NotNull(await db.Locations.FindAsync(1));
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldDelete_WhenLocationIsUnused()
    {
        await using var db = TestDbHelper.GetDbContext();
        db.Locations.AddRange(
            new Location { Id = 1, Name = "Hall A", City = "Sofia" },
            new Location { Id = 2, Name = "Unused Hall", City = "Varna" });
        await db.SaveChangesAsync();
        var controller = new LocationsController(db);
        TestDbHelper.SetUser(controller, "organizer-1");
        TestDbHelper.SetTempData(controller);

        var result = await controller.DeleteConfirmed(2);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(await db.Locations.FindAsync(2));
    }
}
