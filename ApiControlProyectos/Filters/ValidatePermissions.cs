using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Data;
using System.Linq;
using ApiControlProyectos.Data;
using System.Collections.Generic;

namespace ApiControlProyectos.Filters
{
    public class ValidatePermissions: ActionFilterAttribute
    {
        public string PermissionCode { get; set; }
        public int ModueId { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DataAccess da = new DataAccess();
            DataTable dtPermissions = new DataTable();
            var userIdClaim = filterContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals("id", StringComparison.InvariantCultureIgnoreCase)).Value;

            dtPermissions = da.GetDataTable("get_user_item_permission", 
                new List<DBParam> {
                    new DBParam("p_user_id", int.Parse(userIdClaim)),
                    new DBParam("p_code", PermissionCode),
                    new DBParam("p_module_id", ModueId)
                }
            );

            if (dtPermissions.Rows.Count > 0)
            {
                if (dtPermissions.Rows[0]["result"].ToString() != "1")
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.HttpContext.Response.Headers.Clear();
                    filterContext.Result = new EmptyResult();
                }
            }
        }
    }
}
