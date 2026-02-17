using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Account;

public class ChangeUsernameViewModel
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    [Display(Name = "New username")]
    public string NewUsername { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = null!;
}
