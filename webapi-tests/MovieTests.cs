using System.ComponentModel.DataAnnotations;
using System.Linq;
using CloudMovieDatabase.Models;
using Xunit;

namespace CloudMovieDatabase.Tests
{
    public class MovieTests
    {
        [Fact]
        public void CanValidateModel()
        {
            var movie = new Movie("", 4444);
            var validationResults = movie.Validate(new ValidationContext(movie)).ToList();
            Assert.Equal(2, validationResults.Count);
            Assert.Equal("Year cannot be in the future,A movie should have at least one actor", 
                string.Join(",", validationResults.Select(result => result.ErrorMessage)));
        }
    }
}