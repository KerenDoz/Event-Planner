using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.Account;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Username or Email")]
    public string UsernameOrEmail { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}
