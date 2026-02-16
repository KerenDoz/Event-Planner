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
}