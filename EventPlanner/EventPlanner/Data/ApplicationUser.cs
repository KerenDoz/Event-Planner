using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventPlanner.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Data;

public class ApplicationUser : IdentityUser
{
    // [Required]
    // [StringLength(50, MinimumLength = 2)]
    // public string FirstName { get; set; } = null!;

    // [Required]
    // [StringLength(50, MinimumLength = 2)]
    // public string LastName { get; set; } = null!;

    // [NotMapped]
    // public string FullName => $"{FirstName} {LastName}";

    public ICollection<Participant> Participants { get; set; } = new HashSet<Participant>();
    public ICollection<Event> CreatedEvents { get; set; } = new HashSet<Event>();

    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
