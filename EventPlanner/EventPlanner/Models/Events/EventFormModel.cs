using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.Models.Events;

public class EventFormModel : IValidatableObject
{
    [Required]
    [StringLength(80, MinimumLength = 5)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(800, MinimumLength = 20)]
    public string Description { get; set; } = null!;

    [Required]
    [Display(Name = "Start date")]
    public DateTime StartDate { get; set; }

    [Required]
    [Display(Name = "End date")]
    public DateTime EndDate { get; set; }

    [Range(1, 5000)]
    public int Capacity { get; set; }

    [Display(Name = "Public")]
    public bool IsPublic { get; set; } = true;

    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "Location")]
    public int LocationId { get; set; }

    // For dropdowns
    public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Locations { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate < DateTime.Now.AddMinutes(-1))
        {
            yield return new ValidationResult("Start date must be in the future.", new[] { nameof(StartDate) });
        }

        if (EndDate <= StartDate)
        {
            yield return new ValidationResult("End date must be after start date.", new[] { nameof(EndDate) });
        }
    }
}
