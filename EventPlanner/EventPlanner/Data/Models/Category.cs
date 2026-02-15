using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Data.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string Name { get; set; } = null!;
}
