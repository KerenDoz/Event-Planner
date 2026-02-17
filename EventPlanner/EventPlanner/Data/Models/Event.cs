using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using EventPlanner.Data;

namespace EventPlanner.Data.Models;

public class Event
{
    public int Id { get; set; }

    [Required]
    [StringLength(80, MinimumLength = 5)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(800, MinimumLength = 20)]
    public string Description { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(1, 5000)]
    public int Capacity { get; set; }

    public bool IsPublic { get; set; } = true;

    // Relations
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    // Organizer (Identity user)
    [Required]
    public string OrganizerId { get; set; } = null!;
    public ApplicationUser Organizer { get; set; } = null!;
}
