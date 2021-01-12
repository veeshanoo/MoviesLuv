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
    public class MovieController : ControllerBase
    {
        private readonly DbCtx _db;
        private readonly JWTSettings _jwtsettings;

        public MovieController(IOptions<JWTSettings> jwtsettings, DbCtx db)
        {
            _jwtsettings = jwtsettings.Value;
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = UserRole.Everyone)]
        public IActionResult Get()
        {
            return Ok(_db.Movie.ToList());
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Admin)]
        public IActionResult Create(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            movie.NrOfReviews = 0;
            movie.Rating = 0;
            movie.MovieId = new Guid();
            _db.Movie.Add(movie);
            _db.SaveChanges();
            return Ok(movie);
        }

        [HttpPatch]
        [Authorize(Roles = UserRole.Admin)]
        [Route("{id}")]
        public IActionResult Edit(Guid id, Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            movie.NrOfReviews = 0;
            movie.Rating = 0;
            movie.MovieId = new Guid();
            _db.Movie.Add(movie);
            _db.SaveChanges();
            return Ok(movie);
        }
    }
}
