using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesLuv.Models
{
    public class DbCtx : DbContext
    {
        public DbCtx(DbContextOptions<DbCtx> options) : base(options)
        {
            //Database.SetInitializer<DbCtx>(new Initp());
            //Database.SetInitializer<DbCtx>(new CreateDatabaseIfNotExists<DbCtx>());
            //Database.SetInitializer<DbCtx>(new DropCreateDatabaseIfModelChanges<DbCtx>());
            //Database.SetInitializer<DbCtx>(new DropCreateDatabaseAlways<DbCtx>());
        }

        public DbSet<User> User { get; set; }
        public DbSet<Movie> Movie { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<ToWatch> ToWatch { get; set; }
    }
}
