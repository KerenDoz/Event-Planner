using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using EventPlanner.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Data.Models;

public class Event
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(1000, MinimumLength = 20)]
    public string Description { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(1, 10000)]
    public int Capacity { get; set; }

    public bool IsPublic { get; set; } = true;

    // Relations
    [Required]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    [Required]
    public int LocationId { get; set; }

    [ForeignKey(nameof(LocationId))]
    public Location Location { get; set; } = null!;

    // Organizer (Identity user)
    [Required]
    public string OrganizerId { get; set; } = null!;

    [ForeignKey(nameof(OrganizerId))]
    public ApplicationUser Organizer { get; set; } = null!;

    public ICollection<Participant> Participants { get; set; } = new HashSet<Participant>();

}
