using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiControlProyectos.Data
{
    public class DBParam
    {
        public DBParam(string p_name, object p_value)
        {
            this.name = p_name;
            this.value = p_value;
        }
        public string name { get; set; }
        public object value { get; set; }
    }
}
