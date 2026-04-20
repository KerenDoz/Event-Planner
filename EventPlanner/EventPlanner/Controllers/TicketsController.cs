using System.Security.Claims;
using EventPlanner.Data;
using EventPlanner.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class TicketsController : Controller
{
    private readonly ApplicationDbContext db;

    public TicketsController(ApplicationDbContext db)
    {
        this.db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Join(int eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        bool alreadyJoined = await db.Tickets.AnyAsync(t =>
            t.EventId == eventId && t.UserId == userId
        );

        if (!alreadyJoined)
        {
            var ticket = new Ticket { EventId = eventId, UserId = userId! };

            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();
        }

        return RedirectToAction("Details", "Events", new { id = eventId });
    }

    [HttpPost]
    public async Task<IActionResult> Leave(int eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var ticket = await db.Tickets.FirstOrDefaultAsync(t =>
            t.EventId == eventId && t.UserId == userId
        );

        if (ticket != null)
        {
            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();
        }

        return RedirectToAction("Details", "Events", new { id = eventId });
    }
}
