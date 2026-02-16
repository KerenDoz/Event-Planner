using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Locations;

public class LocationFormModel
{
    [Required]
    [StringLength(60, MinimumLength = 3)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string City { get; set; } = null!;

    [StringLength(80)]
    public string? Address { get; set; }
}
