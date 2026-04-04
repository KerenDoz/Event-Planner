using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Data.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string Name { get; set; } = null!;

    public ICollection<Event> Events { get; set; } = new HashSet<Event>();

}
