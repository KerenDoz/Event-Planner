using EventPlanner.Data;
using EventPlanner.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var username = model.Username.Trim();
        var email = model.Email.Trim();

        if (await userManager.FindByNameAsync(username) != null)
        {
            ModelState.AddModelError(nameof(model.Username), "Username is already taken.");
            return View(model);
        }

        if (await userManager.FindByEmailAsync(email) != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Email is already registered.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = username,
            Email = email
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            return View(model);
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        // allow login with username OR email
        var input = model.UsernameOrEmail.Trim();
        ApplicationUser? user = await userManager.FindByNameAsync(input);

        if (user == null)
        {
            user = await userManager.FindByEmailAsync(input);
        }

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(
            user.UserName!,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Manage()
    {
        // var user = await userManager.GetUserAsync(User);
        // if (user == null) return RedirectToAction(nameof(Login));

        // var model = new ManageViewModel
        // {
        //     Username = user.UserName ?? "",
        //     Email = user.Email ?? ""
        // };

        return View(); // return View(model)
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Manage(ManageViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        var newUsername = model.Username.Trim();
        var newEmail = model.Email.Trim();

        // Username uniqueness
        var existingByName = await userManager.FindByNameAsync(newUsername);
        if (existingByName != null && existingByName.Id != user.Id)
        {
            ModelState.AddModelError(nameof(model.Username), "Username is already taken.");
            return View(model);
        }

        // Email uniqueness
        var existingByEmail = await userManager.FindByEmailAsync(newEmail);
        if (existingByEmail != null && existingByEmail.Id != user.Id)
        {
            ModelState.AddModelError(nameof(model.Email), "Email is already in use.");
            return View(model);
        }

        user.UserName = newUsername;
        user.Email = newEmail;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            return View(model);
        }

        // refresh auth cookie so the navbar updates immediately
        await signInManager.RefreshSignInAsync(user);

        TempData["Message"] = "Account updated successfully.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize]
    public async Task<IActionResult> ChangeUsername()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        return View(new ChangeUsernameViewModel { NewUsername = user.UserName ?? "" });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeUsername(ChangeUsernameViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        var newUsername = model.NewUsername.Trim();

        // verify password
        if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Invalid password.");
            return View(model);
        }

        // uniqueness
        var existing = await userManager.FindByNameAsync(newUsername);
        if (existing != null && existing.Id != user.Id)
        {
            ModelState.AddModelError(nameof(model.NewUsername), "Username is already taken.");
            return View(model);
        }

        user.UserName = newUsername;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        TempData["Message"] = "Username updated.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize]
    public async Task<IActionResult> ChangeEmail()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        return View(new ChangeEmailViewModel { NewEmail = user.Email ?? "" });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        var newEmail = model.NewEmail.Trim();

        // verify password
        if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Invalid password.");
            return View(model);
        }

        // uniqueness
        var existing = await userManager.FindByEmailAsync(newEmail);
        if (existing != null && existing.Id != user.Id)
        {
            ModelState.AddModelError(nameof(model.NewEmail), "Email is already in use.");
            return View(model);
        }

        user.Email = newEmail;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        TempData["Message"] = "Email updated.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        TempData["Message"] = "Password updated.";
        return RedirectToAction(nameof(Manage));
    }

}
