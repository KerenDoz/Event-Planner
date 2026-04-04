using System.ComponentModel.DataAnnotations;
using EventPlanner.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Data;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = null!;

    public ICollection<Participant> Participants { get; set; } = new HashSet<Participant>();
    public ICollection<Event> CreatedEvents { get; set; } = new HashSet<Event>();
}