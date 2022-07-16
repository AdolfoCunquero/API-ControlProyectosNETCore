using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    [Table("auth_user")]
    public class AuthUser
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        //[JsonIgnore]
        public string password { get; set; }
        [Required]
        public string first_name { get; set; }
        [Required]
        public string last_name { get; set; }
        [Required]
        public bool is_superuser { get; set; }
        [Required]
        public int status_code { get; set; }

        [JsonIgnore]
        public string usr_creation { get; set; }
        [JsonIgnore]
        public DateTime date_creation { get; set; }
        [JsonIgnore]
        public string usr_updated { get; set; }
        [JsonIgnore]
        public DateTime? date_updated { get; set; }
    }
}
