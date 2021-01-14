using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesLuv.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesLuv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToWatchController : ControllerBase
    {
        private readonly DbCtx _db;
        private readonly JWTSettings _jwtsettings;

        public ToWatchController(IOptions<JWTSettings> jwtsettings, DbCtx db)
        {
            _jwtsettings = jwtsettings.Value;
            _db = db;
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Everyone)]
        [Route("{id}")]
        public IActionResult Create(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Guid userId = new Guid(User.Identity.Name);
            ToWatch toWatch = _db.ToWatch.FirstOrDefault(x => x.MovieId == id && x.UserId == userId);
            if (toWatch != null)
            {
                return Ok(toWatch);
            }
            ToWatch newFavorite = new ToWatch
            {
                ToWatchId = Guid.NewGuid(),
                MovieId = id,
                UserId = userId,
            };
            _db.Add(newFavorite);
            _db.SaveChanges();
            return Ok(toWatch);
        }

        [HttpGet]
        [Authorize(Roles = UserRole.Everyone)]
        public IActionResult Get()
        {
            Guid userId = new Guid(User.Identity.Name);
            List<ToWatch> toWatchList = _db.ToWatch.Where(x => x.UserId == userId).ToList();
            foreach (var favorite in toWatchList)
            {
                favorite.Movie = _db.Movie.Find(favorite.MovieId);
            }
            return Ok(toWatchList);
        }

        [HttpDelete]
        [Authorize(Roles = UserRole.Admin)]
        [Route("{id}")]
        public IActionResult DeleteReview(Guid id)
        {
            var toWatch = _db.ToWatch.Find(id);
            if (toWatch != null)
            {
                _db.ToWatch.Remove(toWatch);
                _db.SaveChanges();
                return Ok();
            }
            return Ok();
        }
    }
}
