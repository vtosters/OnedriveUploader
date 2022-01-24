using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnedriveUploader.Services;

namespace OnedriveUploader.Pages;

[Authorize]
public class Upload : PageModel
{
    private readonly ILogger<Upload> _logger;
    private readonly OnedriveService _onedrive;
    public string DirectUrl { get; set; }

    public Upload(ILogger<Upload> logger, OnedriveService onedrive)
    {
        _logger = logger;
        _onedrive = onedrive;
    }
    
    public IFormFile UploadedFile { get; set; }

    public async Task OnGetAsync()
    {
        
    }
    
    public async Task OnPostAsync()
    {
        if (UploadedFile == null || UploadedFile.Length == 0)
        {
            _logger.LogInformation($"UploadedFile is null.");
            return;
        }

        _logger.LogInformation($"Uploading {UploadedFile.FileName}.");

        using (var stream = new MemoryStream()) {
            await UploadedFile.CopyToAsync(stream);
            DirectUrl = await _onedrive.UploadLargeFile(
                UploadedFile.FileName, stream);
        }

        await OnGetAsync();
    }
}