using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesLuv.Models
{
    public class Movie
    {
        public Guid MovieId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int LaunchYear { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public int NrOfReviews { get; set; }
        [Required]
        public float Rating { get; set; }
        [NotMapped]
        public bool IsFavorite { get; set; }

        public ICollection<Review> Reviews { get; set; }
        public ICollection<ToWatch> ToWatchList { get; set; }
        public ICollection<Favorites> FavoritesList { get; set; }
    }
}
