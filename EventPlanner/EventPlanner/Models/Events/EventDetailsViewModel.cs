namespace EventPlanner.Models.Events;

public class EventDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public bool IsPublic { get; set; }

    public string CategoryName { get; set; } = null!;
    public string LocationName { get; set; } = null!;
    public string City { get; set; } = null!;
    public string? Address { get; set; }

    public string OrganizerUserName { get; set; } = null!;
    public bool IsOwner { get; set; }
}
