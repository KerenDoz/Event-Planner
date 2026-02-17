using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Categories;

public class CategoryFormModel
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string Name { get; set; } = null!;
}
