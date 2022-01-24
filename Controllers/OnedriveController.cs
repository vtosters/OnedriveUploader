using Microsoft.AspNetCore.Mvc;
using OnedriveUploader.Services;

namespace OnedriveUploader.Controllers;

[ApiController]
[Route("api/")]
public class OnedriveController : ControllerBase
{
    private readonly OnedriveService _service;
    private readonly IConfiguration _configuration;

    public OnedriveController(OnedriveService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> PostFormData(string token, [FromForm] IFormFile file)
    {
        if (token != _configuration["UploaderPassword"]) 
            return Unauthorized();
        
        string directUrl;
        using (var sr = new StreamReader(file.OpenReadStream()))
        {
            directUrl = await _service.UploadLargeFile(file.FileName, sr.BaseStream);
            return Ok(directUrl);
        }

        return StatusCode(500);
    }
}