
using Core.Domain.UserContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LauBjuTizVezBra.Pages;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    [BindProperty]
    public string Username { get; set; } = null!;

    [BindProperty]
    public string Password { get; set; } = null!;

    [BindProperty]
    public string ConfirmPassword { get; set; } = null!;
    public IdentityError[] Errors { get; private set; } = Array.Empty<IdentityError>();

    public RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public void OnGet(){}

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            if (Password != ConfirmPassword)
            {
                return Page();
            }
            var user = new User{UserName = Username};
            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Redirect("Index");
            }
            // Get all errors from result.Errors and put them into my Errors list
            Errors = result.Errors.ToArray();
        }

        return Page();
    }
}

