using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiControlProyectos.Models;
using Microsoft.AspNetCore.Authorization;
using ApiControlProyectos.Filters;
using Microsoft.Extensions.Options;

namespace ApiControlProyectos.Controllers
{
    [Route("api/group")]
    [ApiController]
    [Authorize]
    public class AuthGroupController : ControllerBase
    {
        private DBContext _context = new DBContext();
        private readonly AppSettings _appSettings;

        public AuthGroupController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private string getUsername()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("username", StringComparison.InvariantCultureIgnoreCase));
            return userIdClaim.Value.ToString();
        }

        // GET: api/group
        [HttpGet]
        [ValidatePermissions(PermissionCode = "view_groups", ModueId = 2)]
        public ActionResult<object> GetAuthGroups([FromQuery(Name = "page")] string page)
        {
            int valPage;
            Int32.TryParse(page, out valPage);
            valPage = valPage <= 0 ? 1 : valPage;

            var paginationSize = _appSettings.PAGINATION_SIZE;

            var data = (
                    from gr in _context.AuthGroups
                    join cat in _context.Catalogs on gr.status_code equals cat.value
                    where cat.table_name == "auth_group" 
                        && cat.field_name == "status_code" 
                        && cat.status_code == 1
                        && new[] {0, 1}.Contains(gr.status_code)
                    select new
                    {
                        gr.id,
                        gr.name,
                        gr.status_code,
                        status_code_text = cat.description
                    }
                ).Skip((valPage - 1) * paginationSize).Take(paginationSize).ToList();

            int totalItemCount = _context.AuthGroups.Where(i=> new[] {0, 1}.Contains(i.status_code) ).Count();
            var pagination = Convert.ToInt32(Math.Ceiling(totalItemCount / Convert.ToDouble(paginationSize)));

            return new {
                current_page = valPage,
                total_pages = pagination,
                data
            };
        }

        // GET: api/group/5
        [HttpGet("{id}")]
        [ValidatePermissions(PermissionCode = "view_groups", ModueId = 2)]
        public ActionResult<object> GetAuthGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authGroup = (
                    from gr in _context.AuthGroups
                    join cat in _context.Catalogs on gr.status_code equals cat.value
                    where cat.table_name == "auth_group"
                        && cat.field_name == "status_code"
                        && cat.status_code == 1
                        && new[] { 0, 1 }.Contains(gr.status_code)
                        && gr.id == id
                    select new
                    {
                        gr.id,
                        gr.name,
                        gr.status_code,
                        status_code_text = cat.description
                    }).FirstOrDefault();

            if (authGroup == null)
            {
                return NotFound();
            }

            return Ok(authGroup);
        }

        // PUT: api/group/5
        [HttpPut("{id}")]
        [ValidatePermissions(PermissionCode = "edit_group", ModueId = 2)]
        public async Task<IActionResult> PutAuthGroup([FromRoute] int id, [FromBody] AuthGroup authGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != authGroup.id)
            {
                return BadRequest();
            }

            var currentGroup = _context.AuthGroups.Where(i => i.id == id).FirstOrDefault();
            currentGroup.name = authGroup.name;
            currentGroup.date_updated = DateTime.Now;
            currentGroup.usr_updated = getUsername();

            //_context.Entry(authGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthGroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/group
        [HttpPost]
        [ValidatePermissions(PermissionCode = "add_new_group", ModueId = 2)]
        public async Task<IActionResult> PostAuthGroup([FromBody] AuthGroup authGroup)
        {
            authGroup.date_creation = DateTime.Now;
            authGroup.usr_creation = getUsername();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AuthGroups.Add(authGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthGroup", new { id = authGroup.id }, authGroup);
        }

        // DELETE: api/group/5
        [HttpDelete("{id}")]
        [ValidatePermissions(PermissionCode = "delete_group", ModueId = 2)]
        public async Task<IActionResult> DeleteAuthGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authGroup = await _context.AuthGroups.FindAsync(id);
            if (authGroup == null)
            {
                return NotFound();
            }

            //_context.AuthGroups.Remove(authGroup);
            authGroup.usr_updated = getUsername();
            authGroup.date_updated = DateTime.Now;
            authGroup.status_code = 2;

            await _context.SaveChangesAsync();

            return Ok(authGroup);
        }

        private bool AuthGroupExists(int id)
        {
            return _context.AuthGroups.Any(e => e.id == id);
        }
    }
}