using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

        private string GenerateAccessToken(string username, string userRole)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, userRole),
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
        public IActionResult Login(User user)
        {
            if (user == null)
            {
                return Unauthorized();
            }
            User usr = _db.User.FirstOrDefault(x => x.Username == user.Username && x.Password == user.Password);
            if (usr == null)
            {
                return Unauthorized();
            }
            return Ok(GenerateAccessToken(user.Username, "Normal"));

        }
    }
}
