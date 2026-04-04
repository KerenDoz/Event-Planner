using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Data.Models
{
    public class Participant
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;
    }
}