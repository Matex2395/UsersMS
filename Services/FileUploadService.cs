using LoginMS.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LoginMS.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly HttpClient _httpClient;
        private const string CONTENT_TYPE = "image/jpeg";

        public FileUploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadImageAsync(byte[] fileBytes, string fileName)
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(CONTENT_TYPE);
            content.Add(fileContent, "file", fileName);

            _httpClient.Timeout = TimeSpan.FromMinutes(5);

            var response = await _httpClient.PostAsync("https://localhost:7123/api/Files/Upload", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (result != null && result!.url != null)
            {
                return result!.url.ToString();
            }
            else
            {
                throw new Exception($"La URL de la imagen no existe o no se ha generado: {result!.Url}");
            }
        }

        public async Task<string> UploadUserImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException("Archivo No Válido");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            return await UploadImageAsync(fileBytes, file.FileName);
        }
    }
}
