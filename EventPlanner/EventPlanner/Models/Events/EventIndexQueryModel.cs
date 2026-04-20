using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Events;

public class EventIndexQueryModel
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public int? LocationId { get; set; }

    public string? City { get; set; }

    public bool UpcomingOnly { get; set; } = true;

    [DataType(DataType.Date)]
    public DateTime? Date { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> Locations { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<EventListItemViewModel> Events { get; set; } = Enumerable.Empty<EventListItemViewModel>();

    public IEnumerable<SelectListItem> Cities { get; set; } = Enumerable.Empty<SelectListItem>();
}