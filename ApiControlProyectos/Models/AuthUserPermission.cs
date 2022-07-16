using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    [Table("auth_user_permissions")]
    public class AuthUserPermission
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public int permission_id { get; set; }
        [Required]
        public int status_code { get; set; }

        [JsonIgnore]
        public  string usr_creation { get; set; }
        [JsonIgnore]
        public DateTime date_creation { get; set; }
    }
}
