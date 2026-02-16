using EventPlanner.Data;
using EventPlanner.Data.Models;
using EventPlanner.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Controllers;

public class EventsController : Controller
{
    private readonly ApplicationDbContext db; //EF Core database access
    private readonly UserManager<IdentityUser> userManager; //gives the logged-in user info

    public EventsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        this.db = db;
        this.userManager = userManager;
    }

    // ---------------- Helpers ----------------

    // Is the currently logged-in user the creator (organizer) of this event?
    private bool IsOwner(Event ev) 
    {
        var userId = userManager.GetUserId(User);
        return userId != null && ev.OrganizerId == userId;
    }

    // Returns data for a dropdown
    private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
        => await db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync();

    // Creates dropdown option City-Location
    private async Task<IEnumerable<SelectListItem>> GetLocationSelectListAsync()
        => await db.Locations.AsNoTracking()
            .OrderBy(l => l.City).ThenBy(l => l.Name)
            .Select(l => new SelectListItem($"{l.City} — {l.Name}", l.Id.ToString()))
            .ToListAsync();

    // Is this CategoryId actually in the database?
    private Task<bool> CategoryExists(int id) => db.Categories.AnyAsync(c => c.Id == id);
    // Is this LocationId actually in the database?
    private Task<bool> LocationExists(int id) => db.Locations.AnyAsync(l => l.Id == id);
}