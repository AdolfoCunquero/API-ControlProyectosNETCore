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
    [Route("api/user-group")]
    [ApiController]
    [Authorize]
    public class AuthUserGroupController : ControllerBase
    {
        private DBContext _context = new DBContext();
        private readonly AppSettings _appSettings;
        public AuthUserGroupController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private string getUsername()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("username", StringComparison.InvariantCultureIgnoreCase));
            return userIdClaim.Value.ToString();
        }

        // GET: api/AuthUserGroup
        [HttpGet]
        [ValidatePermissions(PermissionCode = "view_asignations_user_group", ModueId = 6)]
        public ActionResult<object> GetAuthUserGroups([FromQuery(Name = "page")] string page)
        {
            int valPage;
            Int32.TryParse(page, out valPage);
            valPage = valPage <= 0 ? 1 : valPage;

            var paginationSize = _appSettings.PAGINATION_SIZE;

            var data = (
                    from ug in _context.AuthUserGroups
                    join usr in _context.AuthUser on ug.user_id equals usr.id
                    join g in _context.AuthGroups on ug.group_id equals g.id
                    where usr.status_code == 1
                        && g.status_code == 1
                    select new
                    {
                        ug.id,
                        ug.user_id,
                        usr.username,
                        usr.first_name,
                        usr.last_name,
                        ug.group_id,
                        g.name
                    }
                ).Skip((valPage - 1) * paginationSize).Take(paginationSize).ToList();

            int totalItemCount = _context.AuthUserGroups.Count();
            var pagination = Convert.ToInt32(Math.Ceiling(totalItemCount / Convert.ToDouble(paginationSize)));

            return new
            {
                current_page = valPage,
                total_pages = pagination,
                data
            };
        }

        // GET: api/AuthUserGroup/5
        [HttpGet("{id}")]
        [ValidatePermissions(PermissionCode = "view_asignations_user_group", ModueId = 6)]
        public async Task<IActionResult> GetAuthUserGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authUserGroup = await _context.AuthUserGroups.FindAsync(id);

            if (authUserGroup == null)
            {
                return NotFound();
            }

            return Ok(authUserGroup);
        }

        // POST: api/AuthUserGroup
        [HttpPost]
        [ValidatePermissions(PermissionCode = "add_new_asignation_user_group", ModueId = 6)]
        public async Task<IActionResult> PostAuthUserGroup([FromBody] AuthUserGroup authUserGroup)
        {
            authUserGroup.usr_creation = getUsername();
            authUserGroup.date_creation = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AuthUserGroups.Add(authUserGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthUserGroup", new { id = authUserGroup.id }, authUserGroup);
        }

        // DELETE: api/AuthUserGroup/5
        [HttpDelete("{id}")]
        [ValidatePermissions(PermissionCode = "delete_asignation_user_group", ModueId = 6)]
        public async Task<IActionResult> DeleteAuthUserGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authUserGroup = await _context.AuthUserGroups.FindAsync(id);
            if (authUserGroup == null)
            {
                return NotFound();
            }

            _context.AuthUserGroups.Remove(authUserGroup);
            await _context.SaveChangesAsync();

            return Ok(authUserGroup);
        }

        private bool AuthUserGroupExists(int id)
        {
            return _context.AuthUserGroups.Any(e => e.id == id);
        }
    }
}