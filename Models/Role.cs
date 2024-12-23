using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LoginMS.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int vli_id { get; set; }
        [Required]
        public required string vls_name { get; set; }
        [Required]
        public required string vls_description { get; set; }
    }
}
