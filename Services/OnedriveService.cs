using Azure.Identity;
using Microsoft.Graph;

namespace OnedriveUploader.Services;

public class OnedriveService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OnedriveService> _logger;

    public OnedriveService(ILogger<OnedriveService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        var scopes = new[] { "Files.ReadWrite.All", "User.Read", "Sites.ReadWrite.All" };
        var credential = new UsernamePasswordCredential(_configuration["Onedrive:Username"],
            _configuration["Onedrive:Password"],
            _configuration["Onedrive:TenantId"],
            _configuration["Onedrive:ClientId"]);
        
        _graphClient = new GraphServiceClient(credential, scopes);
    }
    
    public async Task<string> UploadLargeFile(string itemPath, Stream stream)
    {
        var uploadProps = new DriveItemUploadableProperties
        {
            ODataType = null,
            AdditionalData = new Dictionary<string, object>
            {
                { "@microsoft.graph.conflictBehavior", "replace" }
            }
        };

        // Create the upload session
        var uploadSession = await _graphClient.Me.Drive.Root
            .ItemWithPath(itemPath)
            .CreateUploadSession(uploadProps)
            .Request()
            .PostAsync();

        // Max slice size must be a multiple of 320 KiB
        var maxSliceSize = 320 * 16384;
        var fileUploadTask =
            new LargeFileUploadTask<DriveItem>(uploadSession, stream, maxSliceSize);

        // Create a callback that is invoked after each slice is uploaded
        var progress = new Progress<long>(prog =>
        {
            _logger.LogInformation($"Uploaded {prog} bytes of {stream.Length} bytes");
        });

        try
        {
            // Upload the file
            var uploadResult = await fileUploadTask.UploadAsync(progress);

            if (uploadResult.UploadSucceeded)
            {
                _logger.LogInformation($"Upload complete, item ID: {uploadResult.ItemResponse.Id}");
                
                // Direct link generator
                var permission = await _graphClient.Me.Drive.Items[uploadResult.ItemResponse.Id]
                    .CreateLink("view", "anonymous").Request().PostAsync();
                var oldLink = new Uri(permission.Link.WebUrl);
                var path = oldLink.PathAndQuery;
                var netloc = oldLink.Host;
                var splittedPath = path.Split('/');
                var personalAttr = splittedPath[3];
                var domain = splittedPath[4];
                var shareLink = splittedPath[5];
        
                var directLink =
                    $"https://{netloc}/{personalAttr}/{domain}/_layouts/15/download.aspx?share={shareLink}";
                
                _logger.LogInformation($"Got a direct link for the file: {directLink}");
                return directLink;
            }
            else
            {
                _logger.LogInformation("Upload failed");
            }
        }
        catch (ServiceException ex)
        {
            _logger.LogError($"Error uploading: {ex.ToString()}");
            throw;
        }

        return "";
    }
}