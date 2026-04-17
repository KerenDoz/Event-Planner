using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventPlanner.Data;

namespace EventPlanner.Data.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    // Relations
    [Required]
    public int EventId { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event Event { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
}