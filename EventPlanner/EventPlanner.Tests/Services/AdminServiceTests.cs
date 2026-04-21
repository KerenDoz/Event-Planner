using EventPlanner.Data;
using EventPlanner.Data.Models;
using EventPlanner.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EventPlanner.Tests.Services;

public class AdminServiceTests
{
    [Fact]
    public async Task DeleteEvent_ShouldRemoveEvent_WhenItExists()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new AdminController(TestDbHelper.GetUserManager(), db);

        var result = await controller.DeleteEvent(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminController.Events), redirect.ActionName);
        Assert.Null(await db.Events.FindAsync(1));
    }

    [Fact]
    public async Task DeleteEvent_ShouldReturnNotFound_WhenMissing()
    {
        await using var db = TestDbHelper.GetDbContext();
        var controller = new AdminController(TestDbHelper.GetUserManager(), db);

        var result = await controller.DeleteEvent(123);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteCategory_ShouldRemoveCategory_WhenUnused()
    {
        await using var db = TestDbHelper.GetDbContext();
        db.Categories.Add(new Category { Id = 7, Name = "Unused" });
        await db.SaveChangesAsync();
        var controller = new AdminController(TestDbHelper.GetUserManager(), db);

        var result = await controller.DeleteCategory(7);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminController.Categories), redirect.ActionName);
        Assert.Null(await db.Categories.FindAsync(7));
    }

    [Fact]
    public async Task DeleteLocation_ShouldRemoveLocation_WhenUnused()
    {
        await using var db = TestDbHelper.GetDbContext();
        db.Locations.Add(new Location { Id = 9, Name = "Unused", City = "Varna" });
        await db.SaveChangesAsync();
        var controller = new AdminController(TestDbHelper.GetUserManager(), db);

        var result = await controller.DeleteLocation(9);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminController.Locations), redirect.ActionName);
        Assert.Null(await db.Locations.FindAsync(9));
    }
}
