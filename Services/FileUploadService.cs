using LoginMS.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LoginMS.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly HttpClient _httpClient;
        private const string FOLDER_NAME = "profile-pictures";
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

            var response = await _httpClient.PostAsync($"/api/Images/UploadImage/{FOLDER_NAME}", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<dynamic>(responseContent);

                if (result != null)
                {
                    // Return image URL
                    return result.Url;
                }
                else
                {
                    throw new Exception($"La URL de la imagen no existe o no se ha generado: {result!.Url}");
                }


            }
            else
            {
                throw new Exception($"Error al subir la imagen: {response.ReasonPhrase}");
            }
        }
    }
}
