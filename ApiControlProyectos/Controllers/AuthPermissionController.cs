using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiControlProyectos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ApiControlProyectos.Filters;

namespace ApiControlProyectos.Controllers
{
    [Route("api/permission")]
    [ApiController]
    [Authorize]
    public class AuthPermissionController : ControllerBase
    {
        private DBContext _context = new DBContext();
        private readonly AppSettings _appSettings;
        public AuthPermissionController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // GET: api/permission
        //[HttpGet]
        //[ValidatePermissions(PermissionCode = "view_permissions", ModueId = 5)]
        //public IEnumerable<AuthPermissions> GetAuthPermissions()
        //{
        //    return _context.AuthPermissions;
        //}

        // GET: api/permission/1
        [HttpGet("{moduleId}")]
        [ValidatePermissions(PermissionCode = "view_permissions", ModueId = 5)]
        public ActionResult<object> GetAuthPermissions([FromRoute] int moduleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authPermissions = _context.AuthPermissions.Where(p => p.module_id == moduleId).ToList();

            if (authPermissions == null)
            {
                return NotFound();
            }

            return Ok(authPermissions);
        }

    }
}