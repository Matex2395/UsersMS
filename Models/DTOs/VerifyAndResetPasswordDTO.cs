namespace LoginMS.Models.DTOs
{
    public class VerifyAndResetPasswordDTO
    {
        public required string vls_email { get; set; }
        public required string vls_code { get; set; }
        public required string vls_newpassword { get; set; }
    }
}
