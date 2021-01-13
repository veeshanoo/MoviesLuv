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
    public class ReviewController : ControllerBase
    {
        private readonly DbCtx _db;
        private readonly JWTSettings _jwtsettings;

        public ReviewController(IOptions<JWTSettings> jwtsettings, DbCtx db)
        {
            _jwtsettings = jwtsettings.Value;
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = UserRole.Admin)]
        public IActionResult Get()
        {
            return Ok(_db.Review.ToList());
        }

        [HttpGet]
        [Authorize(Roles = UserRole.Everyone)]
        [Route("{id}")]
        public IActionResult Get(Guid id)
        {
            Guid userId = new Guid(User.Identity.Name);
            Review review = _db.Review.FirstOrDefault(x => x.MovieId == id && x.UserId == userId);
            if (review != null)
            {
                return Ok(review);
            }
            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = UserRole.Everyone)]
        [Route("all/{id}")]
        public IActionResult GetAll(Guid id)
        {
            return Ok(_db.Review.Where(x => x.MovieId == id).ToList());
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Everyone)]
        [Route("{id}")]
        public IActionResult Create(Guid id, Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            review.ReviewId = Guid.NewGuid();
            review.MovieId = id; 
            Guid userId = new Guid(User.Identity.Name);
            review.UserId = userId;

            // Check if review does not exist
            Review checkReview = _db.Review.FirstOrDefault(x => x.MovieId == review.MovieId && x.UserId == review.UserId);
            if (checkReview == null)
            {
                _db.Review.Add(review);

                // Here we update movie itself
                Movie movie = _db.Movie.Find(id);
                movie.Rating = (movie.NrOfReviews * movie.Rating + review.Rating) / (movie.NrOfReviews + 1);
                movie.NrOfReviews += 1;
                _db.Movie.Update(movie);

                _db.SaveChanges();
                return Ok(review);
            }

            return BadRequest("User already posted a review for this movie");
        }

        [HttpDelete]
        [Authorize(Roles = UserRole.Admin)]
        [Route("{id}")]
        public IActionResult DeleteReview(Guid id)
        {
            var review = _db.Review.Find(id);
            if (review != null)
            {
                _db.Review.Remove(review);

                // Here we update movie itself
                Movie movie = _db.Movie.Find(review.MovieId);
                movie.Rating = (movie.NrOfReviews * movie.Rating - review.Rating) / (movie.NrOfReviews - 1);
                movie.NrOfReviews -= 1;
                _db.Movie.Update(movie);

                _db.SaveChanges();
                return Ok();
            }
            return NotFound($"Movie with Id: {id} was not found.");
        }
    }
}
