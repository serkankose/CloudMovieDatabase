using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudMovieDatabase.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        public ICollection<ActorMovie> Actors { get; set; }
    }
}