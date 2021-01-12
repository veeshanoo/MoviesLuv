using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesLuv.Models
{
    public class Review
    {
        public Guid ReviewId { get; set; }

        [Required]
        [ForeignKey("Movie")]
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string Feedback { get; set; }
        [Required]
        public float Rating { get; set; }
    }
}
