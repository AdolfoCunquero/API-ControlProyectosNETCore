using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    [Table("auth_permissions")]
    public class AuthPermissions
    {
        [Key]
        public int id { get; set; }
        public int module_id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }
}
