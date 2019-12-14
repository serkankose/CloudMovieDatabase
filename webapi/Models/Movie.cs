using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudMovieDatabase.Models
{
    public class Movie : IValidatableObject
    {
        public Movie(string title, uint year)
        {
            Title = title;
            Year = year;
        }

        public int Id { get; set; }
        [Required] 
        public string Title { get; set; }
        public uint Year { get; set; }
        public string Genre{ get; set; }

        public virtual ICollection<ActorMovie> Actors { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Year > DateTime.Now.Year)
            {
                yield return new ValidationResult("Year cannot be in the future", new[]{ nameof(Year) });
            }

            if (Actors == null || Actors.Count == 0)
            {
                yield return new ValidationResult("A movie should have at least one actor", new[] { nameof(Actors) });
            }
        }
    }
}