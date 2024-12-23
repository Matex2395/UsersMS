using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LoginMS.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int vli_id { get; set; }
        [Required]
        public required string vls_name { get; set; }
        [Required]
        public required string vls_lastname { get; set; }
        [Required]
        [EmailAddress]
        public required string vls_email { get; set; }
        [Required]
        [MinLength(8)]
        public required string vls_password { get; set; }
        public int vli_points { get; set; } = 0;

        // Navigation 
        public int? vli_role { get; set; }
        public Role? vlo_role { get; set; }
        public int? vli_extrarole { get; set; }
        public Role? vlo_extrarole { get; set; }
    }
}
