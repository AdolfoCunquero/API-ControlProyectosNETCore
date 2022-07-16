using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    [Table("auth_group_permissions")]
    public class AuthGroupPermission
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public int group_id { get; set; }
        [Required]
        public int permission_id { get; set; }
        [Required]
        public int status_code { get; set; }

        [JsonIgnore]
        public string usr_creation { get; set; }
        [JsonIgnore]
        public DateTime date_creation { get; set; }
    }
}
