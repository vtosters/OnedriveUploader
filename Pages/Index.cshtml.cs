using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using OnedriveUploader.Services;

namespace OnedriveUploader.Pages;

public class IndexModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        return RedirectToPage("/Upload");
    }
}