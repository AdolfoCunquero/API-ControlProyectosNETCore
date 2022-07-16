using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    public class LoginRequest
    {
        [Required]
        [DisplayName("username")]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
