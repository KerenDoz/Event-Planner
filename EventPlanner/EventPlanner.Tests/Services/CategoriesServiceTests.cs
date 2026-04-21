using EventPlanner.Controllers;
using EventPlanner.Data.Models;
using EventPlanner.Models.Categories;
using EventPlanner.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Tests.Services;

public class CategoriesServiceTests
{
    [Fact]
    public async Task Create_ShouldTrimName_AndSaveCategory()
    {
        await using var db = TestDbHelper.GetDbContext();
        var controller = new CategoriesController(db);
        var model = new CategoryFormModel { Name = "  Workshops  " };

        var result = await controller.Create(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CategoriesController.Index), redirect.ActionName);
        Assert.Equal("Workshops", db.Categories.Single().Name);
    }

    [Fact]
    public async Task Create_ShouldReturnView_WhenCategoryAlreadyExists()
    {
        await using var db = TestDbHelper.GetDbContext();
        db.Categories.Add(new Category { Name = "Music" });
        await db.SaveChangesAsync();
        var controller = new CategoriesController(db);

        var result = await controller.Create(new CategoryFormModel { Name = " Music " });

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CategoryFormModel>(view.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.Equal("Music", model.Name);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldNotDelete_WhenCategoryIsUsed()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new CategoriesController(db);
        TestDbHelper.SetUser(controller, "organizer-1");
        TestDbHelper.SetTempData(controller);

        var result = await controller.DeleteConfirmed(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CategoriesController.Index), redirect.ActionName);
        Assert.NotNull(controller.TempData["Error"]);
        Assert.NotNull(await db.Categories.FindAsync(1));
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldDelete_WhenCategoryIsUnused()
    {
        await using var db = TestDbHelper.GetDbContext();
        db.Categories.AddRange(new Category { Id = 1, Name = "Music" }, new Category { Id = 2, Name = "Unused" });
        await db.SaveChangesAsync();
        var controller = new CategoriesController(db);
        TestDbHelper.SetUser(controller, "organizer-1");
        TestDbHelper.SetTempData(controller);

        var result = await controller.DeleteConfirmed(2);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(await db.Categories.FindAsync(2));
    }
}
