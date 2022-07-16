using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiControlProyectos.Models;
using ApiControlProyectos.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApiControlProyectos.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private DBContext _context = new DBContext();

        [HttpPost("login")]
        public ActionResult<object> GetToken([FromBody] LoginRequest data)
        {
            
            AES aes = new AES();
            var user = _context.AuthUser.Where(u => u.username == data.username && u.status_code == 1).FirstOrDefault();

            if (user == null)
            {
                return Unauthorized();
            }

            var passwordDecrypt = aes.AESDecrypt(user.password);

            if (data.password == passwordDecrypt)
            {
                var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                var conf = builder.Build();

                //security key
                string securityKey = conf["JWT:SECRET_KEY"];
                int minutesDuration = int.Parse(conf["JWT:DURATION_MIN"]);
                var expires = DateTime.Now.AddMinutes(minutesDuration);
                //symmetric security key
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

                //signing credentials
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                //add claims
                var claims = new List<Claim>();
                //claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
                //claims.Add(new Claim(ClaimTypes.Role, "Reader"));
                claims.Add(new Claim("id", user.id.ToString()));
                claims.Add(new Claim("username", user.username));

                //create token
                var token = new JwtSecurityToken(
                        issuer: "smesk.in",
                        audience: "readers",
                        expires: expires,
                        signingCredentials: signingCredentials
                        , claims: claims
                    );

                JwtResponse res = new JwtResponse();
                res.expires = expires;
                res.token = new JwtSecurityTokenHandler().WriteToken(token);
                return res;
            }else
            {
                return Unauthorized();
            }
        }
    }
}