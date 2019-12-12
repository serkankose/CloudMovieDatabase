using CloudMovieDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudMovieDatabase.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}

        public DbSet<Actor> Actors {get; set;}
    }
}