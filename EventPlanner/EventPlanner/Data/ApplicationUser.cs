using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventPlanner.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Data;

public class ApplicationUser : IdentityUser
{
    public ICollection<Participant> Participants { get; set; } = new HashSet<Participant>();
    public ICollection<Event> CreatedEvents { get; set; } = new HashSet<Event>();

    public ICollection<EventRating> Ratings { get; set; } = new List<EventRating>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
