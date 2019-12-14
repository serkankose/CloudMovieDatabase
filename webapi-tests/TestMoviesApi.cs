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
    public class TestMoviesApi
    {
        [Fact]
        public void CanNOTAddMovieWithoutActors()
        {
            const string databaseName = "movies.CanNOTAddMovieWithoutActors";
            using var controllerContext = CreateDataContext(databaseName);
            var moviesController = new Controllers.MoviesController(controllerContext);
            var postMovie = moviesController
                .PostMovie(new Movie("Fight Club", 2000));

            Assert.IsAssignableFrom<BadRequestObjectResult>(postMovie.Result.Result);
        }

        [Fact]
        public void CanNOTAddMovieInTheFuture()
        {
            const string databaseName = "movies.CanNOTAddMovieInTheFuture";
            using var controllerContext = CreateDataContext(databaseName);
            var moviesController = new Controllers.MoviesController(controllerContext);
            var postMovie = moviesController
                .PostMovie(new Movie("Fight Club", 3293));

            Assert.IsAssignableFrom<BadRequestObjectResult>(postMovie.Result.Result);
        }

        [Fact]
        public void CanAddMovie()
        {
            const string databaseName = "movie.CanAddMovie";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Edward", "Norton"));
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.SaveChanges();


            using var controllerContext = CreateDataContext(databaseName);
            var moviesController = new Controllers.MoviesController(controllerContext);
            var postMovie = moviesController.PostMovie(new Movie("Fight Club", 2000){Actors = new List<ActorMovie>
            {
                new ActorMovie(){ActorId = 1}
            }});

            Assert.IsAssignableFrom<ActionResult<Movie>>(postMovie.Result);

            var value = GetObjectValue(postMovie);

            Assert.Equal("Fight Club", value.Title);
            Assert.Equal(2000u, value.Year);

            static Movie GetObjectValue(Task<ActionResult<Movie>> task)
            {
                return (Movie)(task.Result.Result as CreatedAtActionResult).Value;
            }
        }

        [Fact]
        public void CanUpdateMovie()
        {
            const string databaseName = "movie.CanUpdateMovie";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Lance", "Henriksen"));
            dataContext.SaveChanges();

            var moviesController = new Controllers.MoviesController(CreateDataContext(databaseName));
            var postMovie = moviesController.PostMovie(new Movie("Aliens", 1986)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1}
                }
            });


            var moviesPutController = new Controllers.MoviesController(CreateDataContext(databaseName));
            var putMovie = moviesPutController.PutMovie(postMovie.Id, new Movie("Aliens", 1986)
            {
                Id = postMovie.Id,
                Genre = "Science Fiction",
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1},
                    new ActorMovie(){ActorId = 2}
                }
            });


            Assert.IsAssignableFrom<NoContentResult>(putMovie.Result);
        }

        [Fact]
        public void TryUpdateMovieWithWrongIdReturnsBadRequest()
        {
            const string databaseName = "movie.CanUpdateMovie";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.SaveChanges();

            var moviesController = new Controllers.MoviesController(CreateDataContext(databaseName));
            var postMovie = moviesController.PostMovie(new Movie("Aliens", 1986)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1}
                }
            });


            var moviesPutController = new Controllers.MoviesController(CreateDataContext(databaseName));
            
            //the Id given doesn't match the object's Id
            var wrongId = postMovie.Id + 1;

            var putMovie = moviesPutController.PutMovie(wrongId, new Movie("Aliens", 1986)
            {
                Id = postMovie.Id,
                Genre = "Science Fiction",
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1},
                    new ActorMovie(){ActorId = 2}
                }
            });


            Assert.IsAssignableFrom<BadRequestResult>(putMovie.Result);
        }
        
        [Fact]
        public void TryUpdateNonExistentMovieReturnsNotFound()
        {
            const string databaseName = "movie.CanNotUpdateMovie";

            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.SaveChanges();

            var moviesController = new Controllers.MoviesController(CreateDataContext(databaseName));
            var postMovie = moviesController.PostMovie(new Movie("Aliens", 1986)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1}
                }
            });


            var moviesPutController = new Controllers.MoviesController(CreateDataContext(databaseName));

            const int badId = 9999;

            var putMovie = moviesPutController.PutMovie(badId, new Movie("The Thing", 1986)
            {
                Id = badId,
                Genre = "Science Fiction",
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1},
                    new ActorMovie(){ActorId = 2}
                }
            });


            Assert.IsAssignableFrom<NotFoundResult>(putMovie.Result);
        }

        [Fact]
        public void CanGetMovie()
        {
            const string databaseName = "movie.CanGetMovie";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Lance", "Henriksen"));
            var saveChanges = dataContext.SaveChanges();
            Assert.True(saveChanges > 0);

            var moviesController = new Controllers.MoviesController(CreateDataContext(databaseName));
            var postMovie = moviesController.PostMovie(new Movie("Aliens", 1986)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1},
                    new ActorMovie(){ActorId = 2}
                }
            });


            var actor = moviesController.GetMovie(1);
            Assert.IsAssignableFrom<Movie>(actor.Result.Value);
            var resultValue = actor.Result.Value;
            Assert.NotNull(resultValue);
            Assert.Equal("Aliens", resultValue.Title);
            Assert.Equal(1986u, resultValue.Year);
            Assert.Equal(2, resultValue.Actors.Count);
            Assert.Equal(2, resultValue.Actors.Last().ActorId);
        }

        [Fact]
        public void CanDeleteMovie()
        {
            const string databaseName = "movie.CanDeleteMovie";
            using var dataContext = CreateDataContext(databaseName);
            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Lance", "Henriksen"));
            var saveChanges = dataContext.SaveChanges();
            Assert.True(saveChanges > 0);

            var moviesController = new Controllers.MoviesController(CreateDataContext(databaseName));
            var postMovie = moviesController.PostMovie(new Movie("Aliens", 1986)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1},
                    new ActorMovie(){ActorId = 2}
                }
            });

            const int id = 1;

            var actors = moviesController.DeleteMovie(id);
            Assert.Equal("Aliens", actors.Result.Value.Title);

            var deleted = moviesController.GetMovie(id);
            Assert.IsAssignableFrom<NotFoundResult>(deleted.Result.Result);

        }

        [Fact]
        public void TryDeleteNonExistentMovieReturnsNotFound()
        {
            const string databaseName = "movie.TryDeleteNonExistentMovie";
            using var dataContext = CreateDataContext(databaseName);


            var moviesController = new Controllers.MoviesController(dataContext);

            var movie = moviesController.DeleteMovie(2);
            Assert.IsAssignableFrom<NotFoundResult>(movie.Result.Result);
        }


        [Fact]
        public async void CanGetMovies()
        {
            const string databaseName = "movies.CanGetMovies";
            using var dataContext = CreateDataContext(databaseName);

            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
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
            var getMoviesController = new Controllers.MoviesController(controllerContext);

            var movies = getMoviesController.GetMovies();
            Assert.Equal(2, movies.Result.Value.Count());
        }



        [Fact]
        public async void CanGetMoviesByYear()
        {
            const string databaseName = "movies.CanGetMoviesByYear";
            using var dataContext = CreateDataContext(databaseName);

            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
            dataContext.Actors.Add(new Actor("Alain", "Delon"));
            dataContext.Actors.Add(new Actor("Williem", "Dafoe"));
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
            await moviesController.PostMovie(new Movie("Platoon", 1986)
            {
                Actors = new List<ActorMovie>
                {
                    new ActorMovie(){ActorId = 1},
                    new ActorMovie(){ActorId = 3}
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
            var getMoviesController = new Controllers.MoviesController(controllerContext);

            var movies = getMoviesController.GetMoviesByYear(1986);
            Assert.Equal(2, movies.Result.Value.Count());
        }


        [Fact]
        public async void CanGetMoviesActors()
        {
            const string databaseName = "movie.CanGetActors";
            using var dataContext = CreateDataContext(databaseName);

            dataContext.Actors.Add(new Actor("Sigourney", "Weawer"));
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
            var getMoviesController = new Controllers.MoviesController(controllerContext);

            var movieActors = await getMoviesController.GetMovieActors(1);
            var movieActorsResult = movieActors.Result as dynamic;
            Assert.Equal(2, movieActorsResult.Value.Count);
            Assert.Equal("Weawer", movieActorsResult.Value[0].LastName);
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
