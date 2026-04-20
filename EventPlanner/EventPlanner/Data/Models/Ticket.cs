using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Data.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
