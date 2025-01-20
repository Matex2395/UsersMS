namespace LoginMS.Models.DTOs
{
    public class SearchBarUsersDTO
    {
        public int vli_id { get; set; }
        public required string vls_name { get; set; }
        public required string vls_lastname { get; set; }
        public required string vls_email { get; set; }
    }
}
