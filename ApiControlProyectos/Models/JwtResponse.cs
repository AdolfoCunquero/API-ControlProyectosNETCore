using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    public class JwtResponse
    {
        public string token { get; set; }
        public DateTime expires { get; set; }
    }
}
