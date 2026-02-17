using EventPlanner.Data;
using EventPlanner.Data.Models;
using EventPlanner.Models.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext db;

    public CategoriesController(ApplicationDbContext db)
    {
        this.db = db;
    }

    // PUBLIC
    public async Task<IActionResult> Index()
    {
        var categories = await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    // AUTH
    [Authorize]
    public IActionResult Create()
    {
        return View(new CategoryFormModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormModel model)
    {
        var name = model.Name?.Trim() ?? "";

        if (await db.Categories.AnyAsync(c => c.Name == name))
        {
            ModelState.AddModelError(nameof(model.Name), "This category already exists.");
        }

        if (!ModelState.IsValid)
        {
            model.Name = name;
            return View(model);
        }

        db.Categories.Add(new Category { Name = name });
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // AUTH
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        return View(new CategoryFormModel { Name = category.Name });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormModel model)
    {
        var category = await db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        var name = model.Name?.Trim() ?? "";

        if (await db.Categories.AnyAsync(c => c.Id != id && c.Name == name))
        {
            ModelState.AddModelError(nameof(model.Name), "This category already exists.");
        }

        if (!ModelState.IsValid)
        {
            model.Name = name;
            return View(model);
        }

        category.Name = name;
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // AUTH + SAFE DELETE
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound();

        return View(category);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();

        bool isUsed = await db.Events.AnyAsync(e => e.CategoryId == id);
        if (isUsed)
        {
            TempData["Error"] = "Cannot delete this category because it is used by at least one event.";
            return RedirectToAction(nameof(Index));
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
