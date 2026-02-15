namespace EventPlanner.Models.Events;

public class EventListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public string CategoryName { get; set; } = null!;
    public string City { get; set; } = null!;
}
