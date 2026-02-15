using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Models.Events;

public class EventIndexQueryModel
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public bool UpcomingOnly { get; set; } = true;

    public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<EventListItemViewModel> Events { get; set; } = Enumerable.Empty<EventListItemViewModel>();
}
