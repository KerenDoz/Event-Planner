using System.ComponentModel.DataAnnotations;
using EventPlanner.Controllers;
using EventPlanner.Data;
using EventPlanner.Data.Models;
using EventPlanner.Models.Comments;
using EventPlanner.Models.Events;
using EventPlanner.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Tests.Services;

public class EventsServiceTests
{
    [Fact]
    public async Task Index_ShouldReturnOnlyPublicUpcomingEvents_WhenRequested()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());

        var result = await controller.Index(new EventIndexQueryModel { UpcomingOnly = true });

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EventIndexQueryModel>(view.Model);
        var events = model.Events.ToList();
        Assert.Single(events);
        Assert.Equal("Public Future Music", events[0].Title);
    }

    [Fact]
    public async Task Index_ShouldFilterBySearchAndCity()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());

        var result = await controller.Index(new EventIndexQueryModel
        {
            UpcomingOnly = false,
            Search = "Music",
            City = "Sofia"
        });

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EventIndexQueryModel>(view.Model);
        Assert.Single(model.Events);
        Assert.Equal(1, model.Events.Single().Id);
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_ForPrivateEvent_WhenAnonymous()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller);

        var result = await controller.Details(2);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ShouldPopulateRatingsTicketsAndComments()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        db.EventRatings.AddRange(
            new EventRating { EventId = 1, UserId = "organizer-1", Value = 5 },
            new EventRating { EventId = 1, UserId = "user-2", Value = 3 });
        db.Tickets.Add(new Ticket { EventId = 1, UserId = "user-2" });
        db.Comments.Add(new Comment { EventId = 1, UserId = "user-2", Content = "Great event!" });
        await db.SaveChangesAsync();
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.Details(1);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EventDetailsViewModel>(view.Model);
        Assert.Equal(4, model.AverageRating);
        Assert.Equal(2, model.RatingsCount);
        Assert.Equal(3, model.UserRating);
        Assert.Equal(1, model.TicketsCount);
        Assert.True(model.IsJoined);
        Assert.Single(model.Comments);
    }

    [Fact]
    public async Task Create_ShouldReturnView_WhenCategoryOrLocationIsInvalid()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "organizer-1", "organizer");

        var model = new EventFormModel
        {
            Title = "Test title",
            Description = "Valid description",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(1).AddHours(2),
            Capacity = 10,
            CategoryId = 999,
            LocationId = 999,
        };

        var result = await controller.Create(model);

        var view = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<EventFormModel>(view.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.NotEmpty(returnedModel.Categories);
        Assert.NotEmpty(returnedModel.Locations);
    }

    [Fact]
    public async Task Create_ShouldAddEvent_WhenModelIsValid()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "organizer-1", "organizer");

        var model = new EventFormModel
        {
            Title = "New conference",
            Description = "A valid business event description",
            StartDate = DateTime.Now.AddDays(10),
            EndDate = DateTime.Now.AddDays(10).AddHours(4),
            Capacity = 120,
            IsPublic = true,
            CategoryId = 1,
            LocationId = 1,
        };

        var result = await controller.Create(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(EventsController.Details), redirect.ActionName);
        Assert.Contains(db.Events, e => e.Title == "New conference" && e.OrganizerId == "organizer-1");
    }

    [Fact]
    public async Task Edit_ShouldReturnForbid_WhenUserIsNotOwner()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.Edit(1, new EventFormModel
        {
            Title = "Changed",
            Description = "Changed description",
            StartDate = DateTime.Now.AddDays(2),
            EndDate = DateTime.Now.AddDays(2).AddHours(1),
            Capacity = 20,
            CategoryId = 1,
            LocationId = 1,
        });

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldReturnForbid_WhenUserIsNotOwner()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.DeleteConfirmed(1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task MyEvents_ShouldReturnOnlyCurrentUsersEvents()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "organizer-1", "organizer");

        var result = await controller.MyEvents();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EventIndexQueryModel>(view.Model);
        Assert.Equal(2, model.Events.Count());
        Assert.All(model.Events, e => Assert.Contains(e.Id, new[] { 1, 2 }));
    }

    [Fact]
    public async Task AddComment_ShouldAddComment_WhenInputIsValid()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.AddComment(new AddCommentViewModel { EventId = 1, Content = " Nice one! " });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(EventsController.Details), redirect.ActionName);
        Assert.Equal("Nice one!", db.Comments.Single().Content);
    }

    [Fact]
    public async Task AddComment_ShouldReturnNotFound_WhenEventDoesNotExist()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.AddComment(new AddCommentViewModel { EventId = 999, Content = "Hi" });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Rate_ShouldIgnoreOutOfRangeValues()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.Rate(1, 7);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(EventsController.Details), redirect.ActionName);
        Assert.Empty(db.EventRatings);
    }

    [Fact]
    public async Task Rate_ShouldCreateNewRating_WhenMissing()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.Rate(1, 4);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(4, db.EventRatings.Single().Value);
    }

    [Fact]
    public async Task Rate_ShouldUpdateExistingRating_WhenPresent()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        db.EventRatings.Add(new EventRating { EventId = 1, UserId = "user-2", Value = 2 });
        await db.SaveChangesAsync();
        var controller = new EventsController(db, TestDbHelper.GetUserManager());
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.Rate(1, 5);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(db.EventRatings);
        Assert.Equal(5, db.EventRatings.Single().Value);
    }

    [Fact]
    public async Task TicketJoinAndLeave_ShouldAddAndRemoveTicket()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        var controller = new TicketsController(db);
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var joinResult = await controller.Join(1);
        Assert.IsType<RedirectToActionResult>(joinResult);
        Assert.Single(db.Tickets);

        var leaveResult = await controller.Leave(1);
        Assert.IsType<RedirectToActionResult>(leaveResult);
        Assert.Empty(db.Tickets);
    }

    [Fact]
    public async Task TicketJoin_ShouldNotDuplicateTicket()
    {
        await using var db = TestDbHelper.GetDbContext();
        await TestDbHelper.SeedBasicDataAsync(db);
        db.Tickets.Add(new Ticket { EventId = 1, UserId = "user-2" });
        await db.SaveChangesAsync();
        var controller = new TicketsController(db);
        TestDbHelper.SetUser(controller, "user-2", "guest");

        var result = await controller.Join(1);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(db.Tickets);
    }

    [Fact]
    public void EventEntity_ShouldFailValidation_WhenEndDateIsBeforeStartDate()
    {
        var entity = new Event
        {
            Title = "Title",
            Description = "Valid description",
            StartDate = DateTime.Now.AddDays(2),
            EndDate = DateTime.Now.AddDays(1),
            Capacity = 10,
            CategoryId = 1,
            LocationId = 1,
            OrganizerId = "organizer-1"
        };

        var results = ValidateModel(entity);

        Assert.Contains(results, r => r.ErrorMessage == "End date must be after start date.");
    }

    [Fact]
    public void EventFormModel_ShouldFailValidation_ForPastStartDate_AndWrongEndDate()
    {
        var model = new EventFormModel
        {
            Title = "Title",
            Description = "Valid description",
            StartDate = DateTime.Now.AddHours(-2),
            EndDate = DateTime.Now.AddHours(-3),
            Capacity = 10,
            CategoryId = 1,
            LocationId = 1,
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.ErrorMessage == "Start date must be in the future.");
        Assert.Contains(results, r => r.ErrorMessage == "End date must be after start date.");
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
