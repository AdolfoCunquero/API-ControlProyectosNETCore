using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    [Table("auth_module")]
    public class AuthModule
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
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
