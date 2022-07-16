using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiControlProyectos.Models;
using Microsoft.AspNetCore.Authorization;
using ApiControlProyectos.Security;
using Microsoft.Extensions.Options;
using ApiControlProyectos.Filters;

namespace ApiControlProyectos.Controllers
{
    [Route("api/user-permission")]
    [ApiController]
    [Authorize]
    public class AuthUserPermissionController : ControllerBase
    {
        private DBContext _context = new DBContext();
        private readonly AppSettings _appSettings;

        public AuthUserPermissionController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private string getUsername()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("username", StringComparison.InvariantCultureIgnoreCase));
            return userIdClaim.Value.ToString();
        }

        // GET: api/AuthUserPermission
        [HttpGet]
        [ValidatePermissions(PermissionCode = "view_user_permissions", ModueId = 2)]
        public ActionResult<object> GetAuthUserPermissions([FromQuery(Name = "page")] string page)
        {
            int valPage;
            Int32.TryParse(page, out valPage);
            valPage = valPage <= 0 ? 1 : valPage;

            var paginationSize = _appSettings.PAGINATION_SIZE;

            var data = (
                    from up in _context.AuthUserPermissions
                    join usr in _context.AuthUser on up.user_id equals usr.id
                    join p in _context.AuthPermissions on up.permission_id equals p.id
                    where 
                        usr.status_code == 1
                    select new
                    {
                        up.id,
                        up.user_id,
                        usr.username,
                        usr.first_name,
                        usr.last_name,
                        up.permission_id,
                        p.description
                    }
                ).Skip((valPage - 1) * paginationSize).Take(paginationSize).ToList();

            int totalItemCount = _context.AuthUserPermissions.Count();
            var pagination = Convert.ToInt32(Math.Ceiling(totalItemCount / Convert.ToDouble(paginationSize)));

            return new
            {
                current_page = valPage,
                total_pages = pagination,
                data
            };

        }

        // GET: api/AuthUserPermission/5
        [HttpGet("{id}")]
        [ValidatePermissions(PermissionCode = "view_user_permissions", ModueId = 2)]
        public async Task<IActionResult> GetAuthUserPermission([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authUserPermission = await _context.AuthUserPermissions.FindAsync(id);

            if (authUserPermission == null)
            {
                return NotFound();
            }

            return Ok(authUserPermission);
        }

        // POST: api/AuthUserPermission
        [HttpPost]
        [ValidatePermissions(PermissionCode = "add_new_user_permission", ModueId = 2)]
        public async Task<IActionResult> PostAuthUserPermission([FromBody] AuthUserPermission authUserPermission)
        {
            authUserPermission.usr_creation = getUsername();
            authUserPermission.date_creation = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AuthUserPermissions.Add(authUserPermission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthUserPermission", new { id = authUserPermission.id }, authUserPermission);
        }

        // DELETE: api/AuthUserPermission/5
        [HttpDelete("{id}")]
        [ValidatePermissions(PermissionCode = "delete_user_permission", ModueId = 2)]
        public async Task<IActionResult> DeleteAuthUserPermission([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authUserPermission = await _context.AuthUserPermissions.FindAsync(id);
            if (authUserPermission == null)
            {
                return NotFound();
            }

            _context.AuthUserPermissions.Remove(authUserPermission);
            await _context.SaveChangesAsync();

            return Ok(authUserPermission);
        }

        private bool AuthUserPermissionExists(int id)
        {
            return _context.AuthUserPermissions.Any(e => e.id == id);
        }
    }
}