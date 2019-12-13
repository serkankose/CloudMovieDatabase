using System;
using System.Diagnostics;
using System.Linq;
using Xunit;
using NSubstitute;
using CloudMovieDatabase.Data;
using CloudMovieDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudMovieDatabase.Tests
{
    public class TestActorsApi
    {
        [Fact]
        public void AddOne()
        {
            DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestMovieDb")
                .Options;


            using var dataContext = new DataContext(options);
            
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();


            var actorsController = new Controllers.ActorsController(new DataContext(options));
            var actors = actorsController.GetActors();
            Assert.Equal(2, actors.Result.Value.Count());
            
        }
    }
}
