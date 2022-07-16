using ApiControlProyectos.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ApiControlProyectos.Data
{
    public class DataAccess
    {
        private DBContext ctx = new DBContext();
        public DataTable GetDataTable(string procedureName, List<DBParam> parameters)
        {
            DataTable dt = new DataTable();
            using (var conn = ctx.Database.GetDbConnection())
            {
                conn.Open();
                DbCommand cmdItems = conn.CreateCommand();
                cmdItems.CommandText = procedureName;
                cmdItems.CommandType = CommandType.StoredProcedure;
                foreach(DBParam par in parameters)
                {
                    cmdItems.Parameters.Add(new MySqlParameter(par.name, par.value));
                }
                
                var dr = cmdItems.ExecuteReader();
                dt.Load(dr);
                conn.Close();
            }
            return dt;
        }
    }
}
