using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Models
{
    public class AppSettings
    {
        public string MySqlConnection { get; set; }
        public int PAGINATION_SIZE { get; set; }
    }
}
