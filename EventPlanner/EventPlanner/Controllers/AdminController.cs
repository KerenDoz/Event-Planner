using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventPlanner.Data;
using EventPlanner.Data.Models;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ApplicationDbContext context;

    public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        this.userManager = userManager;
        this.context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    //  USERS 
    public IActionResult Users()
    {
        var users = userManager.Users.ToList();
        return View(users);
    }

    public async Task<IActionResult> Suspend(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        user.LockoutEnd = DateTime.UtcNow.AddDays(30);
        await userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> SuspendPermanent(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        user.LockoutEnd = DateTime.UtcNow.AddYears(100);
        await userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Unsuspend(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        user.LockoutEnd = null;
        await userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Users));
    }

    //  EVENTS 
    public IActionResult Events()
    {
        var events = context.Events.ToList();
        return View(events);
    }

    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await context.Events.FindAsync(id);

        if (ev == null)
        {
            return NotFound();
        }

        context.Events.Remove(ev);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Events));
    }

    //  CATEGORIES 
    public IActionResult Categories()
    {
        var categories = context.Categories.ToList();
        return View(categories);
    }

    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        bool isUsed = await context.Events.AnyAsync(e => e.CategoryId == id);
        if (isUsed)
        {
            TempData["ErrorMessage"] = "Category cannot be deleted because it is used by existing events.";
            return RedirectToAction(nameof(Categories));
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Categories));
    }

    //  LOCATIONS 
    public IActionResult Locations()
    {
        var locations = context.Locations.ToList();
        return View(locations);
    }

    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await context.Locations.FindAsync(id);

        if (location == null)
        {
            return NotFound();
        }

        bool isUsed = await context.Events.AnyAsync(e => e.LocationId == id);
        if (isUsed)
        {
            TempData["ErrorMessage"] = "Location cannot be deleted because it is used by existing events.";
            return RedirectToAction(nameof(Locations));
        }

        context.Locations.Remove(location);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Locations));
    }
}