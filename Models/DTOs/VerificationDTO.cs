namespace LoginMS.Models.DTOs
{
    public class VerificationDTO
    {
        public required string vls_email { get; set; }
        public required string vls_code { get; set; }
    }
}
