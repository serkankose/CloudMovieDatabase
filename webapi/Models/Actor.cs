using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CloudMovieDatabase.Models
{
    public class Actor
    {
        public Actor() { }

        public Actor(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public int Id { get; set; }
        
        [Timestamp] public byte[] Timestamp { get; set; }
        
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public ICollection<ActorMovie> Movies {get; }

        public IList<(int, string, uint)> Filmography => Movies?
            .OrderBy(actorMovie => actorMovie.Movie.Year)
            .Select((movie, i) => (i + 1, movie.Movie.Title, movie.Movie.Year)).ToList();

    }
}
