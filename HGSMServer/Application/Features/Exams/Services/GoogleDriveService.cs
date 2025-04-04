using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HGSMAPI
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly string _folderId;

        public GoogleDriveService(IConfiguration configuration)
        {
            var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "service-account-credentials.json");
            var credential = GoogleCredential.FromFile(credentialPath)
                .CreateScoped(DriveService.Scope.DriveFile);
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "HGSMServer"
            });

            _folderId = "1cJ9S4PXMpXe99lF57coEXodkNNPIqBU1"; // ID thư mục Google Drive
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                MimeType = file.ContentType,
                Parents = new List<string> { _folderId }
            };

            using var stream = file.OpenReadStream();
            var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
            request.Fields = "id";

            var upload = await request.UploadAsync();
            if (upload.Status != UploadStatus.Completed)
                throw new Exception("Upload failed");

            var fileId = request.ResponseBody.Id;
            var permission = new Google.Apis.Drive.v3.Data.Permission
            {
                Type = "anyone",
                Role = "reader"
            };
            await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

            return $"https://drive.google.com/uc?id={fileId}";
        }
    }
}