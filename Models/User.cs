using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesLuv.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        [Required]
        public string UserRole { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<Review> Reviews { get; set; }
        public ICollection<ToWatch> ToWatchList { get; set; }
        public ICollection<Favorites> FavoritesList { get; set; }
    }
}
