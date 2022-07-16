using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiControlProyectos.Models;
using Microsoft.AspNetCore.Authorization;
using ApiControlProyectos.Security;
using ApiControlProyectos.Filters;
using Microsoft.Extensions.Options;

namespace ApiControlProyectos.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class AuthUserController : ControllerBase
    {
        private DBContext _context = new DBContext();
        private readonly AppSettings _appSettings;

        public AuthUserController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private string getUsername()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("username", StringComparison.InvariantCultureIgnoreCase));
            return userIdClaim.Value.ToString();
        }

        // GET: api/AuthUser
        [HttpGet]
        [ValidatePermissions(PermissionCode = "view_users", ModueId = 1)]
        public ActionResult<object> GetAuthUser([FromQuery(Name = "page")] string page)
        {
            int valPage;
            Int32.TryParse(page, out valPage);
            valPage = valPage <= 0 ? 1 : valPage;

            var paginationSize = _appSettings.PAGINATION_SIZE;

            var data = (
                    from usr in _context.AuthUser
                    join cat in _context.Catalogs on usr.status_code equals cat.value
                    where cat.table_name == "auth_user"
                        && cat.field_name == "status_code"
                        && cat.status_code == 1
                        && new[] {0, 1}.Contains(usr.status_code)
                    select new
                    {
                        usr.id,
                        usr.username,
                        usr.first_name,
                        usr.last_name,
                        usr.status_code,
                        status_code_text = cat.description
                    }
                ).Skip((valPage - 1) * paginationSize).Take(paginationSize).ToList();

            int totalItemCount = _context.AuthUser.Where(i => new[] { 0, 1 }.Contains(i.status_code)).Count();
            var pagination = Convert.ToInt32(Math.Ceiling(totalItemCount / Convert.ToDouble(paginationSize)));

            return new
            {
                current_page = valPage,
                total_pages = pagination,
                data
            };
        }

        // GET: api/AuthUser/5

        [HttpGet("{id}")]
        [ValidatePermissions(PermissionCode = "view_users", ModueId = 1)]
        public ActionResult<object> GetAuthUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authUser = (
                    from usr in _context.AuthUser
                    join cat in _context.Catalogs on usr.status_code equals cat.value
                    where cat.table_name == "auth_user"
                        && cat.field_name == "status_code"
                        && cat.status_code == 1
                        && usr.id == id
                        && new[] { 0, 1 }.Contains(usr.status_code)
                    select new
                    {
                        usr.id,
                        usr.username,
                        usr.first_name,
                        usr.last_name,
                        usr.status_code,
                        status_code_text = cat.description
                    }
                ).FirstOrDefault();

            if (authUser == null)
            {
                return NotFound();
            }

            return Ok(authUser);
        }

        // PUT: api/AuthUser/5
        [HttpPut("{id}")]
        [ValidatePermissions(PermissionCode = "edit_user", ModueId = 1)]
        public async Task<IActionResult> PutAuthUser([FromRoute] int id, [FromBody] AuthUser authUser)
        {

            authUser.usr_updated = "acunqueroc";
            authUser.date_updated = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != authUser.id)
            {
                return BadRequest();
            }

            authUser.date_updated = DateTime.Now;
            authUser.usr_updated = getUsername();

            _context.Entry(authUser).State = EntityState.Modified;
            _context.Entry(authUser).Property(p => p.usr_creation).IsModified = false;
            _context.Entry(authUser).Property(p => p.date_creation).IsModified = false;
            _context.Entry(authUser).Property(p => p.password).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthUserExists(id))
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

        // POST: api/AuthUser
        [HttpPost]
        [ValidatePermissions(PermissionCode = "add_new_user", ModueId = 1)]
        public async Task<IActionResult> PostAuthUser([FromBody] AuthUser authUser)
        {
            authUser.usr_creation = "acunqueroc";
            authUser.date_creation = DateTime.Now;

            AES aes = new AES();
            authUser.password = aes.AESEncrypt(authUser.password);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AuthUser.Add(authUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthUser", new { id = authUser.id }, authUser);
        }

        // DELETE: api/AuthUser/5
        [HttpDelete("{id}")]
        [ValidatePermissions(PermissionCode = "delete_user", ModueId = 1)]
        public async Task<IActionResult> DeleteAuthUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authUser = await _context.AuthUser.FindAsync(id);
            if (authUser == null)
            {
                return NotFound();
            }

            //_context.AuthUser.Remove(authUser);

            authUser.status_code = 2;
            authUser.date_updated = DateTime.Now;
            authUser.usr_updated = getUsername();

            _context.Entry(authUser).State = EntityState.Modified;
            _context.Entry(authUser).Property(p => p.usr_creation).IsModified = false;
            _context.Entry(authUser).Property(p => p.date_creation).IsModified = false;
            _context.Entry(authUser).Property(p => p.password).IsModified = false;

            

            await _context.SaveChangesAsync();

            return Ok(authUser);
        }

        private bool AuthUserExists(int id)
        {
            return _context.AuthUser.Any(e => e.id == id);
        }
    }
}