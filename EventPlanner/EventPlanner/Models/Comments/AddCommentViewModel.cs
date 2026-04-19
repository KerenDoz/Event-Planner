using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Comments
{
    public class AddCommentViewModel
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = null!;
    }
}
