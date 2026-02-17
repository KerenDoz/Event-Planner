using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Account;

public class ChangeEmailViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "New email")]
    public string NewEmail { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = null!;
}
