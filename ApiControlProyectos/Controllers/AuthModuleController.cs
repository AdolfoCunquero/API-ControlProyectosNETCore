using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiControlProyectos.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using ApiControlProyectos.Filters;

namespace ApiControlProyectos.Controllers
{
    [Route("api/module")]
    [ApiController]
    [Authorize]
    public class AuthModuleController : ControllerBase
    {
        private DBContext _context = new DBContext();
        private readonly AppSettings _appSettings;

        public AuthModuleController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // GET: api/AuthModule
        [HttpGet]
        [ValidatePermissions(PermissionCode = "view_modules", ModueId = 4)]
        public IEnumerable<AuthModule> GetAuthModule()
        {
            return _context.AuthModule.ToList();
        }

        // GET: api/AuthModule/5
        [HttpGet("{id}")]
        [ValidatePermissions(PermissionCode = "view_modules", ModueId = 4)]
        public async Task<IActionResult> GetAuthModule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authModule = await _context.AuthModule.FindAsync(id);

            if (authModule == null)
            {
                return NotFound();
            }

            return Ok(authModule);
        }

    }
}