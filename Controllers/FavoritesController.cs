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
    public class FavoritesController : ControllerBase
    {
        private readonly DbCtx _db;
        private readonly JWTSettings _jwtsettings;

        public FavoritesController(IOptions<JWTSettings> jwtsettings, DbCtx db)
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
            Favorites favorite = _db.Favorites.FirstOrDefault(x => x.MovieId == id && x.UserId == userId);
            if (favorite != null)
            {
                return Ok(favorite);
            }
            Favorites newFavorite = new Favorites
            {
                FavoritesId = Guid.NewGuid(),
                MovieId = id,
                UserId = userId,
            };
            _db.Add(newFavorite);
            _db.SaveChanges();
            return Ok(favorite);
        }

        [HttpGet]
        [Authorize(Roles = UserRole.Everyone)]
        public IActionResult Get()
        {
            Guid userId = new Guid(User.Identity.Name);
            List<Favorites> favorites = _db.Favorites.Where(x => x.UserId == userId).ToList();
            foreach (var favorite in favorites)
            {
                favorite.Movie = _db.Movie.Find(favorite.MovieId);
            }
            return Ok(favorites);
        }

        [HttpDelete]
        [Authorize(Roles = UserRole.Admin)]
        [Route("{id}")]
        public IActionResult DeleteReview(Guid id)
        {
            var favorite = _db.Favorites.Find(id);
            if (favorite != null)
            {
                _db.Favorites.Remove(favorite);
                _db.SaveChanges();
                return Ok();
            }
            return Ok();
        }
    }
}
