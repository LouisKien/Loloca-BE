namespace Loloca_BE.Business.Services
{
    public interface IGoogleDriveService
    {
        Task UploadFileAsync(Stream fileStream, Google.Apis.Drive.v3.Data.File fileMetadata);
        Task DeleteFileAsync(string fileName, string parentFolderId);
        Task<byte[]> GetFileContentAsync(string fileName, string parentFolderId);
    }
}
