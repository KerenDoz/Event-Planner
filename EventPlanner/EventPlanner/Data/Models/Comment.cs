using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
