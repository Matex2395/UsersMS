namespace LoginMS.Models.DTOs
{
    public class UpdateUserDTO
    {
        public int vli_id { get; set; }
        public required string vls_name { get; set; }
        public required string vls_lastname { get; set; }
        public required string vls_email { get; set; }
        public required string vls_password { get; set; }

        public IFormFile? vlf_image { get; set; }
        public required string vls_role { get; set; }
        public string? vls_extrarole { get; set; }
    }
}
