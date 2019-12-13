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
            using var controllerContext = CreateDataContext("CanAddActor");
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
            using var dataContext = CreateDataContext("CanUpdateActor");
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();

            var actorsController = new Controllers.ActorsController(CreateDataContext("CanUpdateActor"));
            var actor = new Actor("Alain Fabien Maurice Marcel", "Delon"){Id = 2};
            var putActor = actorsController.PutActor(actor.Id, actor);

            Assert.IsAssignableFrom<NoContentResult>(putActor.Result);
        }

        [Fact]
        public void TryUpdateActorWithWrongIdReturnsBadRequest()
        {
            using var dataContext = CreateDataContext("CanUpdateActor");
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();

            var actorsController = new Controllers.ActorsController(CreateDataContext("CanUpdateActor"));
            var actor = new Actor("Monica", "Belluci") { Id = 4 };
            var putActor = actorsController.PutActor(actor.Id + 5, actor);

            Assert.IsAssignableFrom<BadRequestResult>(putActor.Result);
        }
        
        [Fact]
        public void TryUpdateNonExistentActorReturnsNotFound()
        {
            using var dataContext = CreateDataContext("CanUpdateActor");
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();

            var actorsController = new Controllers.ActorsController(CreateDataContext("CanUpdateActor"));
            var actor = new Actor("Monica", "Belluci") { Id = 4 };
            var putActor = actorsController.PutActor(actor.Id, actor);

            Assert.IsAssignableFrom<NotFoundResult>(putActor.Result);
        }

        [Fact]
        public void CanGetActor()
        {
            using var dataContext = CreateDataContext("CanGetActor");
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            var saveChanges = dataContext.SaveChanges();
            Assert.True(saveChanges > 0);

            var actorsController = new Controllers.ActorsController(dataContext);

            var actor = actorsController.GetActor(2);
            Assert.IsAssignableFrom<Actor>(actor.Result.Value);
            var resultValue = actor.Result.Value;
            Assert.NotNull(resultValue);
            Assert.Equal("Alain", resultValue.FirstName);
        }

        [Fact]
        public void CanDeleteActor()
        {
            using var dataContext = CreateDataContext("CanDeleteActor");

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
            using var dataContext = CreateDataContext("TryDeleteNonExistentActor");


            var actorsController = new Controllers.ActorsController(dataContext);

            var actors = actorsController.DeleteActor(2);
            Assert.IsAssignableFrom<NotFoundResult>(actors.Result.Result);
        }


        [Fact]
        public void CanGetActors()
        {
            using var dataContext = CreateDataContext("CanGetActors");

            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();


            using var controllerContext = CreateDataContext("CanGetActors");
            var actorsController = new Controllers.ActorsController(controllerContext);

            var actors = actorsController.GetActors();
            Assert.Equal(2, actors.Result.Value.Count());
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
