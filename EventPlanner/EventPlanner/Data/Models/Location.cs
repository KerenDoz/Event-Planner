using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Data.Models;

public class Location
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string City { get; set; } = null!;

    [StringLength(200)]
    public string? Address { get; set; }

    public ICollection<Event> Events { get; set; } = new HashSet<Event>();

}
