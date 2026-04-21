using System.Security.Claims;
using EventPlanner.Data;
using EventPlanner.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventPlanner.Tests.Helpers;

public static class TestDbHelper
{
    public static ApplicationDbContext GetDbContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public static UserManager<ApplicationUser> GetUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new UserManager<ApplicationUser>(
            store.Object,
            null,
            new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<ApplicationUser>>>().Object);
    }

    public static void SetUser(Controller controller, string? userId = null, string? userName = null)
    {
        var claims = new List<Claim>();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            claims.Add(new Claim(ClaimTypes.Name, userName));
        }

        var identity = claims.Any()
            ? new ClaimsIdentity(claims, "TestAuthType")
            : new ClaimsIdentity();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }

    public static void SetTempData(Controller controller)
    {
        controller.TempData = new TempDataDictionary(
            controller.HttpContext,
            Mock.Of<ITempDataProvider>());
    }

    public static async Task SeedBasicDataAsync(ApplicationDbContext db, string organizerId = "organizer-1")
    {
        var category = new Category { Id = 1, Name = "Music" };
        var category2 = new Category { Id = 2, Name = "Tech" };
        var location = new Location { Id = 1, Name = "Hall A", City = "Sofia", Address = "Center" };
        var location2 = new Location { Id = 2, Name = "Arena", City = "Plovdiv", Address = "Main" };
        var organizer = new ApplicationUser { Id = organizerId, UserName = "organizer", Email = "org@test.com" };
        var otherUser = new ApplicationUser { Id = "user-2", UserName = "guest", Email = "guest@test.com" };

        db.Categories.AddRange(category, category2);
        db.Locations.AddRange(location, location2);
        db.Users.AddRange(organizer, otherUser);

        db.Events.AddRange(
            new Event
            {
                Id = 1,
                Title = "Public Future Music",
                Description = "A public future music event",
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(5).AddHours(2),
                Capacity = 50,
                IsPublic = true,
                CategoryId = 1,
                LocationId = 1,
                OrganizerId = organizerId,
            },
            new Event
            {
                Id = 2,
                Title = "Private Future Tech",
                Description = "A private tech event",
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(3).AddHours(2),
                Capacity = 100,
                IsPublic = false,
                CategoryId = 2,
                LocationId = 2,
                OrganizerId = organizerId,
            },
            new Event
            {
                Id = 3,
                Title = "Public Past Music",
                Description = "A public past music event",
                StartDate = DateTime.Now.AddDays(-3),
                EndDate = DateTime.Now.AddDays(-3).AddHours(2),
                Capacity = 70,
                IsPublic = true,
                CategoryId = 1,
                LocationId = 2,
                OrganizerId = "user-2",
            }
        );

        await db.SaveChangesAsync();
    }
}
