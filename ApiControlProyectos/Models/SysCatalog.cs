using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    [Table("catalog")]
    public class SysCatalog
    {
        
        [Required]
        public string table_name { get; set; }
        
        [Required]
        public string field_name { get; set; }
        
        [Required]
        public int value { get; set; }

        [Required]
        public string description { get; set; }
        [Required]
        public int status_code { get; set; }
        [Required]
        public int is_editable { get; set; }

        [JsonIgnore]
        public string usr_creation { get; set; }
        [JsonIgnore]
        public DateTime date_creation { get; set; }
        [JsonIgnore]
        public string usr_updated { get; set; }
        [JsonIgnore]
        public DateTime date_updated { get; set; }
    }
}
