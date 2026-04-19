using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventPlanner.Data;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Data.Models;

public class Event : IValidatableObject
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

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow; // Experiment

    public bool IsDeleted { get; set; } = false;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
        {
            yield return new ValidationResult(
                "End date must be after start date.",
                new[] { nameof(EndDate) }
            );
        }
    }

    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
