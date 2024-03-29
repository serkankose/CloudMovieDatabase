using CloudMovieDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudMovieDatabase.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}

        public DbSet<Actor> Actors {get; set;}
        public DbSet<Movie> Movies {get; set;}

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>()
                .Ignore(actor => actor.Filmography);
            
            modelBuilder.Entity<Movie>()
                .HasIndex(movie => new {movie.Title, movie.Year})
                .IsUnique();

            modelBuilder.Entity<Actor>()
                .HasIndex(actor => new { actor.FirstName, actor.LastName, actor.Birthday })
                .IsUnique();

            modelBuilder.Entity<ActorMovie>(builder => builder.HasKey(actorMovie => new {actorMovie.ActorId, actorMovie.MovieId}));

            modelBuilder.Entity<ActorMovie>()
                .HasOne(x => x.Actor)
                .WithMany(m => m.Movies)
                .HasForeignKey(x => x.ActorId);

            modelBuilder.Entity<ActorMovie>()
                .HasOne(x => x.Movie)
                .WithMany(e => e.Actors)
                .HasForeignKey(x => x.MovieId);
        }
    }
}
