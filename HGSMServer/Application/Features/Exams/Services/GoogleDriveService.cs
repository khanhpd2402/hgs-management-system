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
        private readonly string _rootFolderId;

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

            _rootFolderId = "1cJ9S4PXMpXe99lF57coEXodkNNPIqBU1"; // ID thư mục gốc
        }

        private async Task<string> GetOrCreateFolderAsync(string folderName, string parentFolderId)
        {
            var query = $"'{parentFolderId}' in parents and mimeType = 'application/vnd.google-apps.folder' and name = '{folderName}' and trashed = false";
            var request = _driveService.Files.List();
            request.Q = query;
            request.Fields = "files(id, name)";
            var result = await request.ExecuteAsync();

            if (result.Files.Any())
            {
                return result.Files.First().Id;
            }

            var folderMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { parentFolderId }
            };

            var createRequest = _driveService.Files.Create(folderMetadata);
            createRequest.Fields = "id";
            var folder = await createRequest.ExecuteAsync();
            return folder.Id;
        }

        public async Task<string> UploadWordFileAsync(IFormFile file, int subjectId, int grade, string subjectName)
        {
            var subjectFolderId = await GetOrCreateFolderAsync(subjectName, _rootFolderId);
            var gradeFolderName = $"Khối {grade}";
            var gradeFolderId = await GetOrCreateFolderAsync(gradeFolderName, subjectFolderId);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                MimeType = file.ContentType,
                Parents = new List<string> { gradeFolderId }
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