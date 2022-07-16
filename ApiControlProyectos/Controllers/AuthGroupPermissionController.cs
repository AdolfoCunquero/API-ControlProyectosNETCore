using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiControlProyectos.Models;
using ApiControlProyectos.Security;
using Microsoft.AspNetCore.Authorization;
using ApiControlProyectos.Filters;
using Microsoft.Extensions.Options;

namespace ApiControlProyectos.Controllers
{
    [Route("api/group-permission")]
    [ApiController]
    [Authorize]
    public class AuthGroupPermissionController : ControllerBase
    {
        private DBContext _context = new DBContext();
        private readonly AppSettings _appSettings;

        public AuthGroupPermissionController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private string getUsername()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("username", StringComparison.InvariantCultureIgnoreCase));
            return userIdClaim.Value.ToString();
        }

        // GET: api/group-permission
        [HttpGet]
        [ValidatePermissions(PermissionCode = "view_group_permissions", ModueId = 3)]
        public ActionResult<object> GetAuthGroupPermissions([FromQuery(Name = "page")] string page)
        {
            int valPage;
            Int32.TryParse(page, out valPage);
            valPage = valPage <= 0 ? 1 : valPage;

            var paginationSize = _appSettings.PAGINATION_SIZE;

            var data = (
                    from gp in _context.AuthGroupPermissions
                    join cat in _context.Catalogs on gp.status_code equals cat.value
                    join gr in _context.AuthGroups on gp.group_id equals gr.id
                    join per in _context.AuthPermissions on gp.permission_id equals per.id
                    where cat.table_name == "auth_group_permissions"
                        && cat.field_name == "status_code"
                        && cat.status_code == 1
                        && new[] {0, 1}.Contains(gr.status_code)
                        && new[] {0, 1}.Contains(gp.status_code)
                    select new
                    {
                        gp.id,
                        gp.group_id,
                        group_name = gr.name,
                        gp.permission_id,
                        permission_description = per.description,
                        gp.status_code,
                        status_code_text = cat.description
                    }
                ).Skip((valPage - 1) * paginationSize).Take(paginationSize).ToList();

            int totalItemCount = _context.AuthGroupPermissions.Where(i => new[] { 0, 1 }.Contains(i.status_code)).Count();
            var pagination = Convert.ToInt32(Math.Ceiling(totalItemCount / Convert.ToDouble(paginationSize)));

            return new
            {
                current_page = valPage,
                total_pages = pagination,
                data
            };
        }

        // GET: api/group-permission/5
        [HttpGet("{id}")]
        [ValidatePermissions(PermissionCode = "view_group_permissions", ModueId = 3)]
        public async Task<IActionResult> GetAuthGroupPermission([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authGroupPermission = await _context.AuthGroupPermissions.FindAsync(id);

            if (authGroupPermission == null)
            {
                return NotFound();
            }

            return Ok(authGroupPermission);
        }

        // POST: api/group-permission
        [HttpPost]
        [ValidatePermissions(PermissionCode = "add_new_group_permission", ModueId = 3)]
        public async Task<IActionResult> PostAuthGroupPermission([FromBody] AuthGroupPermission authGroupPermission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            authGroupPermission.usr_creation = getUsername();
            authGroupPermission.date_creation = DateTime.Now;
            _context.AuthGroupPermissions.Add(authGroupPermission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthGroupPermission", new { id = authGroupPermission.id }, authGroupPermission);
        }

        // DELETE: api/group-permission/5
        [HttpDelete("{id}")]
        [ValidatePermissions(PermissionCode = "delete_group_permission", ModueId = 3)]
        public async Task<IActionResult> DeleteAuthGroupPermission([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authGroupPermission = await _context.AuthGroupPermissions.FindAsync(id);
            if (authGroupPermission == null)
            {
                return NotFound();
            }

            _context.AuthGroupPermissions.Remove(authGroupPermission);
            await _context.SaveChangesAsync();

            return Ok(authGroupPermission);
        }

        private bool AuthGroupPermissionExists(int id)
        {
            return _context.AuthGroupPermissions.Any(e => e.id == id);
        }
    }
}