using EventPlanner.Data;
using EventPlanner.Data.Models;
using EventPlanner.Models.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Controllers;

public class LocationsController : Controller
{
    private readonly ApplicationDbContext db;

    public LocationsController(ApplicationDbContext db)
    {
        this.db = db;
    }

    // PUBLIC
    public async Task<IActionResult> Index()
    {
        var locations = await db.Locations
            .AsNoTracking()
            .OrderBy(l => l.City)
            .ThenBy(l => l.Name)
            .ToListAsync();

        return View(locations);
    }

    // AUTH
    [Authorize]
    public IActionResult Create()
    {
        return View(new LocationFormModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LocationFormModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var location = new Location
        {
            Name = model.Name,
            City = model.City,
            Address = model.Address
        };

        db.Locations.Add(location);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // AUTH
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var location = await db.Locations.FindAsync(id);
        if (location == null) return NotFound();

        var model = new LocationFormModel
        {
            Name = location.Name,
            City = location.City,
            Address = location.Address
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LocationFormModel model)
    {
        var location = await db.Locations.FindAsync(id);
        if (location == null) return NotFound();

        if (!ModelState.IsValid) return View(model);

        location.Name = model.Name;
        location.City = model.City;
        location.Address = model.Address;

        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // AUTH + SAFE DELETE
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await db.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location == null) return NotFound();

        return View(location);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var location = await db.Locations.FirstOrDefaultAsync(l => l.Id == id);
        if (location == null) return NotFound();

        bool isUsed = await db.Events.AnyAsync(e => e.LocationId == id);
        if (isUsed)
        {
            TempData["Error"] = "Cannot delete this location because it is used by at least one event.";
            return RedirectToAction(nameof(Index));
        }

        db.Locations.Remove(location);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
