namespace LoginMS.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadImageAsync(byte[] fileBytes, string fileName);
        Task<string> UploadUserImageAsync(IFormFile file);
    }
}
