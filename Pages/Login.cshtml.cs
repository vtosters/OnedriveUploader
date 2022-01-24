using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnedriveUploader.Pages;

[AllowAnonymous]
public class Login : PageModel
{
    private readonly IConfiguration _configuration;

    public Login(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public string? Password { get; set; }
    public string? Message { get; set; }
    public async Task<IActionResult> OnPostAsync()
    {
        if (Password == _configuration["UploaderPassword"])
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "admin")
            };
            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity));
            return RedirectToPage("/Upload");
        }

        Message = "ПОшёл нахуй ";
        return Page();
    }
}