using System.Security.Claims;
using EventPlanner.Data;
using EventPlanner.Data.Models;
using EventPlanner.Models.Comments;
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
    private readonly UserManager<ApplicationUser> userManager; //gives the logged-in user info

    public EventsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        this.db = db;
        this.userManager = userManager;
    }

    // PUBLIC: Browse
    public async Task<IActionResult> Index([FromQuery] EventIndexQueryModel query)
    {
        query.Categories = await GetCategorySelectListAsync();
        query.Cities = await GetCitySelectListAsync();

        IQueryable<EventPlanner.Data.Models.Event> eventsQuery = db
            .Events.AsNoTracking()
            .Where(e => e.IsPublic)
            .Include(e => e.Category)
            .Include(e => e.Location);

        if (query.UpcomingOnly)
        {
            eventsQuery = eventsQuery.Where(e => e.StartDate >= DateTime.Now);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            eventsQuery = eventsQuery.Where(e => e.Title.Contains(s));
        }

        if (query.CategoryId.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.CategoryId == query.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.City))
        {
            eventsQuery = eventsQuery.Where(e => e.Location.City == query.City);
        }

        query.Events = await eventsQuery
            .OrderBy(e => e.StartDate)
            .Select(e => new EventListItemViewModel
            {
                Id = e.Id,
                Title = e.Title,
                StartDate = e.StartDate,
                CategoryName = e.Category.Name,
                City = e.Location.City,
            })
            .ToListAsync();

        return View(query);
    }

    // PUBLIC: Details
    public async Task<IActionResult> Details(int id)
    {
        var ev = await db
            .Events.AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Location)
            .Include(e => e.Organizer)
            .Include(e => e.Comments)
                .ThenInclude(c => c.User)
            .Include(e => e.Ratings)
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null || (!ev.IsPublic && User.Identity?.IsAuthenticated != true))
        {
            return NotFound();
        }

        var currentUserId = userManager.GetUserId(User);

        var vm = new EventDetailsViewModel
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            StartDate = ev.StartDate,
            EndDate = ev.EndDate,
            Capacity = ev.Capacity,
            IsPublic = ev.IsPublic,
            CategoryName = ev.Category.Name,
            LocationName = ev.Location.Name,
            City = ev.Location.City,
            Address = ev.Location.Address,
            OrganizerUserName = ev.Organizer.UserName ?? ev.Organizer.Email ?? "Unknown",
            IsOwner = currentUserId != null && ev.OrganizerId == currentUserId,

            AverageRating = ev.Ratings.Any() ? ev.Ratings.Average(r => r.Value) : 0,
            RatingsCount = ev.Ratings.Count,
            UserRating = currentUserId != null
                ? ev.Ratings.FirstOrDefault(r => r.UserId == currentUserId)?.Value
                : null,

            TicketsCount = ev.Tickets.Count,
            IsJoined = currentUserId != null && ev.Tickets.Any(t => t.UserId == currentUserId),

            Comments = ev.Comments
                .OrderByDescending(c => c.CreatedOn)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.User.UserName ?? c.User.Email ?? "Unknown",
                    Content = c.Content,
                    CreatedOn = c.CreatedOn,
                })
                .ToList(),
        };

        return View(vm);
    }

    // AUTH: Create
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var now = DateTime.Now;

        var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
        var end = start.AddHours(2);

        var model = new EventFormModel
        {
            StartDate = start,
            EndDate = end,
            Capacity = 1,
            Categories = await GetCategorySelectListAsync(),
            Locations = await GetLocationSelectListAsync(),
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventFormModel model)
    {
        if (!await CategoryExists(model.CategoryId))
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Invalid category.");
        }
        if (!await LocationExists(model.LocationId))
        {
            ModelState.AddModelError(nameof(model.LocationId), "Invalid location.");
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategorySelectListAsync();
            model.Locations = await GetLocationSelectListAsync();
            return View(model);
        }

        var userId = userManager.GetUserId(User)!;

        var ev = new Event
        {
            Title = model.Title,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Capacity = model.Capacity,
            IsPublic = model.IsPublic,
            CategoryId = model.CategoryId,
            LocationId = model.LocationId,
            OrganizerId = userId,
        };

        db.Events.Add(ev);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = ev.Id });
    }

    // AUTH + OWNER: Edit
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var ev = await db.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (ev == null)
            return NotFound();

        if (!IsOwner(ev))
            return Forbid();

        var model = new EventFormModel
        {
            Title = ev.Title,
            Description = ev.Description,
            StartDate = ev.StartDate,
            EndDate = ev.EndDate,
            Capacity = ev.Capacity,
            IsPublic = ev.IsPublic,
            CategoryId = ev.CategoryId,
            LocationId = ev.LocationId,
            Categories = await GetCategorySelectListAsync(),
            Locations = await GetLocationSelectListAsync(),
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EventFormModel model)
    {
        var ev = await db.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (ev == null)
            return NotFound();

        if (!IsOwner(ev))
            return Forbid();

        if (!await CategoryExists(model.CategoryId))
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Invalid category.");
        }
        if (!await LocationExists(model.LocationId))
        {
            ModelState.AddModelError(nameof(model.LocationId), "Invalid location.");
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategorySelectListAsync();
            model.Locations = await GetLocationSelectListAsync();
            return View(model);
        }

        ev.Title = model.Title;
        ev.Description = model.Description;
        ev.StartDate = model.StartDate;
        ev.EndDate = model.EndDate;
        ev.Capacity = model.Capacity;
        ev.IsPublic = model.IsPublic;
        ev.CategoryId = model.CategoryId;
        ev.LocationId = model.LocationId;

        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    // AUTH + OWNER: Delete
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await db
            .Events.AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Location)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound();
        if (!IsOwner(ev))
            return Forbid();

        // reuse details viewmodel for confirm page
        var vm = new EventDetailsViewModel
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            StartDate = ev.StartDate,
            EndDate = ev.EndDate,
            Capacity = ev.Capacity,
            IsPublic = ev.IsPublic,
            CategoryName = ev.Category.Name,
            LocationName = ev.Location.Name,
            City = ev.Location.City,
            Address = ev.Location.Address,
            OrganizerUserName = "",
            IsOwner = true,
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ev = await db.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (ev == null)
            return NotFound();

        if (!IsOwner(ev))
            return Forbid();

        db.Events.Remove(ev);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> MyEvents()
    {
        var userId = userManager.GetUserId(User)!;

        var myEvents = await db
            .Events.AsNoTracking()
            .Where(e => e.OrganizerId == userId)
            .Include(e => e.Category)
            .Include(e => e.Location)
            .OrderBy(e => e.StartDate)
            .Select(e => new EventListItemViewModel
            {
                Id = e.Id,
                Title = e.Title,
                StartDate = e.StartDate,
                CategoryName = e.Category.Name,
                City = e.Location.City,
            })
            .ToListAsync();

        var model = new EventIndexQueryModel
        {
            UpcomingOnly = false, // show all MyEvent by default
            Categories = await GetCategorySelectListAsync(),
            Events = myEvents,
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(AddCommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Details), new { id = model.EventId });
        }

        var eventExists = await db.Events.AnyAsync(e => e.Id == model.EventId);
        if (!eventExists)
        {
            return NotFound();
        }

        var userId = userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var comment = new Comment
        {
            EventId = model.EventId,
            UserId = userId!,
            Content = model.Content.Trim(),
            CreatedOn = DateTime.UtcNow,
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = model.EventId });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rate(int eventId, int value)
    {
        if (value < 1 || value > 5)
        {
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        var userId = userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var evExists = await db.Events.AnyAsync(e => e.Id == eventId);
        if (!evExists)
        {
            return NotFound();
        }

        var rating = await db.EventRatings.FirstOrDefaultAsync(r =>
            r.EventId == eventId && r.UserId == userId
        );

        if (rating == null)
        {
            rating = new EventRating
            {
                EventId = eventId,
                UserId = userId,
                Value = value
            };

            db.EventRatings.Add(rating);
        }
        else
        {
            rating.Value = value;
        }

        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = eventId });
    }

    // ---------------- Helpers ----------------

    // Is the currently logged-in user the creator (organizer) of this event?
    private bool IsOwner(Event ev)
    {
        var userId = userManager.GetUserId(User);
        return userId != null && ev.OrganizerId == userId;
    }

    // Returns data for a dropdown
    private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync() =>
        await db
            .Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync();

    // Creates dropdown option City-Location
    private async Task<IEnumerable<SelectListItem>> GetLocationSelectListAsync() =>
        await db
            .Locations.AsNoTracking()
            .OrderBy(l => l.City)
            .ThenBy(l => l.Name)
            .Select(l => new SelectListItem($"{l.City} — {l.Name}", l.Id.ToString()))
            .ToListAsync();

    // Is this CategoryId actually in the database?
    private Task<bool> CategoryExists(int id) => db.Categories.AnyAsync(c => c.Id == id);

    // Is this LocationId actually in the database?
    private Task<bool> LocationExists(int id) => db.Locations.AnyAsync(l => l.Id == id);

    private async Task<IEnumerable<SelectListItem>> GetCitySelectListAsync() =>
        await db
            .Locations.AsNoTracking()
            .Select(l => l.City)
            .Distinct()
            .OrderBy(c => c)
            .Select(c => new SelectListItem { Value = c, Text = c })
            .ToListAsync();
}
