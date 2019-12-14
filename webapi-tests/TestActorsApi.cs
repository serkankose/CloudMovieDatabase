using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using CloudMovieDatabase.Data;
using CloudMovieDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudMovieDatabase.Tests
{
    public class TestActorsApi
    {
        [Fact]
        public void CanAddActor()
        {
            const string databaseName = "actor.CanAddActor";
            using var controllerContext = CreateDataContext(databaseName);
            var actorsController = new Controllers.ActorsController(controllerContext);
            Task<ActionResult<Actor>> postActor = actorsController.PostActor(new Actor("Sharon", "Stone"));

            Assert.IsAssignableFrom<ActionResult<Actor>>(postActor.Result);

            var value = GetObjectValue(postActor);

            Assert.Equal("Sharon", value.FirstName);
            Assert.Equal("Stone", value.LastName);

            static Actor GetObjectValue(Task<ActionResult<Actor>> task)
            {
                return (Actor)(task.Result.Result as CreatedAtActionResult).Value;
            }
        }

        [Fact]
        public void CanUpdateActor()
        {
            const string databaseName = "actor.CanUpdateActor";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();

            var actorsController = new Controllers.ActorsController(CreateDataContext(databaseName));
            var actor = new Actor("Alain Fabien Maurice Marcel", "Delon"){Id = 2};
            var putActor = actorsController.PutActor(actor.Id, actor);

            Assert.IsAssignableFrom<NoContentResult>(putActor.Result);
        }

        [Fact]
        public void TryUpdateActorWithWrongIdReturnsBadRequest()
        {
            const string databaseName = "actor.CanUpdateActor";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();

            var actorsController = new Controllers.ActorsController(CreateDataContext(databaseName));
            var actor = new Actor("Monica", "Belluci") { Id = 4 };
            var putActor = actorsController.PutActor(actor.Id + 5, actor);

            Assert.IsAssignableFrom<BadRequestResult>(putActor.Result);
        }
        
        [Fact]
        public void TryUpdateNonExistentActorReturnsNotFound()
        {
            const string databaseName = "actor.CanUpdateActor";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();

            var actorsController = new Controllers.ActorsController(CreateDataContext(databaseName));
            var actor = new Actor("Monica", "Belluci") { Id = 4 };
            var putActor = actorsController.PutActor(actor.Id, actor);

            Assert.IsAssignableFrom<NotFoundResult>(putActor.Result);
        }

        [Fact]
        public void CanGetActor()
        {
            const string databaseName = "actor.CanGetActor";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            var birthday = DateTime.Now.Date;
            dataContext.Actors.Add(new Actor("Alain", "Delon"){Birthday = birthday});
            var saveChanges = dataContext.SaveChanges();
            Assert.True(saveChanges > 0);

            var actorsController = new Controllers.ActorsController(dataContext);

            var actor = actorsController.GetActor(2);
            Assert.IsAssignableFrom<Actor>(actor.Result.Value);
            var resultValue = actor.Result.Value;
            Assert.NotNull(resultValue);
            Assert.Equal("Alain", resultValue.FirstName);
            Assert.Equal("Delon", resultValue.LastName);
            Assert.Equal(birthday, resultValue.Birthday);
        }

        [Fact]
        public void CanDeleteActor()
        {
            const string databaseName = "actor.CanDeleteActor";
            using var dataContext = CreateDataContext(databaseName);

            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();



            var actorsController = new Controllers.ActorsController(dataContext);

            const int id = 2;

            var actors = actorsController.DeleteActor(id);
            Assert.Equal("Alain", actors.Result.Value.FirstName);

            var deleted = actorsController.GetActor(id);
            Assert.IsAssignableFrom<NotFoundResult>(deleted.Result.Result);

        }

        [Fact]
        public void TryDeleteNonExistentActorReturnsNotFound()
        {
            const string databaseName = "actor.TryDeleteNonExistentActor";
            using var dataContext = CreateDataContext(databaseName);

            var actorsController = new Controllers.ActorsController(dataContext);

            var actors = actorsController.DeleteActor(2);
            Assert.IsAssignableFrom<NotFoundResult>(actors.Result.Result);
        }


        [Fact]
        public void CanGetActors()
        {
            const string databaseName = "actor.CanGetActors";
            using var dataContext = CreateDataContext(databaseName);

            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();


            using var controllerContext = CreateDataContext(databaseName);
            var actorsController = new Controllers.ActorsController(controllerContext);

            var actors = actorsController.GetActors();
            Assert.Equal(2, actors.Result.Value.Count());
        }

        [Fact]
        public async void CanGetActorsMovies()
        {
            const string databaseName = "actor.CanGetMovies";
            using var dataContext = CreateDataContext(databaseName);

            var sigaurney = new Actor("Sigourney", "Weawer");
            dataContext.Actors.Add(sigaurney);
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();

            var moviesController = new Controllers.MoviesController(dataContext);
            var postMovie = await moviesController.PostMovie(new Movie("Aliens", 1986)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1},
                    new ActorMovie(){ActorId = 2}
                }
            });

            await moviesController.PostMovie(new Movie("Avatar", 2009)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1},
                }
            });


            using var controllerContext = CreateDataContext(databaseName);
            var actorsController = new Controllers.ActorsController(controllerContext);

            var movieActors = await actorsController.GetActorMovies(1);
            var movieActorsResult = movieActors.Result as dynamic;
            Assert.Equal(2, movieActorsResult.Value.Count);
            Assert.Equal("Avatar", movieActorsResult.Value[1].Title);
            var valueTuple = (1, "Aliens", 1986u);
            Assert.Equal(new List<(int, string, uint)> {valueTuple, (2, "Avatar", 2009u)}, sigaurney.Filmography);
        }


        [Fact]
        public async void CanLinkActorsWithMovie()
        {
            const string databaseName = "actor.CanLinkActorsWithMovie";
            using var dataContext = CreateDataContext(databaseName);

            var sigourney = new Actor("Sigourney", "Weawer");
            var lance = new Actor("Lance", "Herliksen");

            dataContext.Actors.Add(sigourney);
            dataContext.Actors.Add(lance);

            dataContext.SaveChanges();

            var moviesController = new Controllers.MoviesController(CreateDataContext(databaseName));
            var movie = new Movie("Aliens", 1986)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = sigourney.Id}
                }
            };
            var postMovie = await moviesController.PostMovie(movie);

            await new Controllers.ActorsController(dataContext)
                .LinkActorMovie(lance.Id, movie.Id);

            var actorsController = new Controllers.ActorsController(CreateDataContext(databaseName));

            var movieActors = await actorsController.GetActorMovies(lance.Id);
            var movieActorsResult = movieActors.Result as dynamic;
            Assert.Equal(1, movieActorsResult.Value.Count);
            Assert.Equal("Aliens", movieActorsResult.Value[0].Title);
        }

        private static DataContext CreateDataContext(string databaseName)
        {
            var options = CreateDbContextOptions(databaseName);
            return new DataContext(options);
        }

        private static DbContextOptions<DataContext> CreateDbContextOptions(string databaseName)
        {
            DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
            return options;
        }
    }
}
