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
            return Ok(_db.Movie.ToList().OrderByDescending(x => x.Rating));
        }

        [HttpGet]
        [Authorize(Roles = UserRole.Everyone)]
        [Route("{id}")]
        public IActionResult Get(Guid id)
        {
            Movie movie = _db.Movie.Find(id);
            return Ok(movie);
        }

        [HttpDelete]
        [Authorize(Roles = UserRole.Admin)]
        [Route("{id}")]
        public IActionResult DeleteMovie(Guid id)
        {
            var movie = _db.Movie.Find(id);
            if (movie != null)
            {
                _db.Movie.Remove(movie);
                _db.SaveChanges();
                return Ok();
            }
            return NotFound($"Movie with Id: {id} was not found.");
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
            movie.MovieId = Guid.NewGuid();
            _db.Movie.Add(movie);
            _db.SaveChanges();
            return Ok(movie);
        }

        [HttpPut]
        [Authorize(Roles = UserRole.Admin)]
        [Route("{id}")]
        public IActionResult Edit(Guid id, Movie newMovie)
        {
            var existingMovie = _db.Movie.Find(id);
            if (existingMovie == null)
            {
                return NotFound($"Movie with Id: {id} was not found.");
            }
            existingMovie.Name = newMovie.Name;
            existingMovie.LaunchYear = newMovie.LaunchYear;
            existingMovie.Summary = newMovie.Summary;
            existingMovie.ImageUrl = newMovie.ImageUrl;
            _db.Movie.Update(existingMovie);
            _db.SaveChanges();
            return Ok(existingMovie);
        }
    }
}
