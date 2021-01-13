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
        [Authorize(Roles = UserRole.Everyone)]
        [Route("{id}")]
        public IActionResult Get(Guid id)
        {
            Review review = _db.Review.Find(id);
            return Ok(review);
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
    }
}
