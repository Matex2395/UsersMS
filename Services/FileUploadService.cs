using LoginMS.Interfaces;
using Newtonsoft.Json;
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

            var response = await _httpClient.PostAsync($"https://localhost:7145/api/Images/UploadImage/{FOLDER_NAME}", content);
            if (response.IsSuccessStatusCode)
            {
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
            else
            {
                throw new Exception($"Error al subir la imagen: {response.ReasonPhrase}");
            }
        }

        public async Task<string> UploadUserImageAsync(IFormFile file)
        {
            // Verify if the file is valid
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException("Archivo No Válido");
            }

            // Convert file into a byte array
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            // Call the Google Cloud Storage microservice to upload image
            string imageUrl = await UploadImageAsync(fileBytes, file.FileName);

            // Return the URL
            return imageUrl;
        }
    }
}
