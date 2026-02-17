using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Account;

public class ManageViewModel
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
