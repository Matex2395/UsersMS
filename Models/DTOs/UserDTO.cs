using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LoginMS.Models.DTOs
{
    public class UserDTO
    {
        public int vli_id { get; set; }
        public required string vls_name { get; set; }
        public required string vls_lastname { get; set; }
        public required string vls_email { get; set; }
        public required string vls_password { get; set; }
        public int vli_points { get; set; } = 0;
        public required string vls_role { get; set; }
        public string? vls_extrarole { get; set; }
    }
}
