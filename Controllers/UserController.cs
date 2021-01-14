using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoviesLuv.Domain;
using MoviesLuv.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoviesLuv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbCtx _db;
        private readonly JWTSettings _jwtsettings;

        public UserController(IOptions<JWTSettings> jwtsettings, DbCtx db)
        {
            _jwtsettings = jwtsettings.Value;
            _db = db;
        }

        private string GenerateAccessToken(Guid id, string userRole, string name)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, id.ToString()),
                    new Claim(ClaimTypes.Role, userRole),
                    new Claim(ClaimTypes.GivenName, name),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginRequest req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (req == null)
            {
                return Unauthorized();
            }
            User dbUser = _db.User.FirstOrDefault(x => x.Username == req.Username && x.Password == req.Password);
            if (dbUser == null)
            {
                return Unauthorized();
            }
            return Ok(GenerateAccessToken(dbUser.UserId, dbUser.UserRole, dbUser.Name));
        }
    }
}
