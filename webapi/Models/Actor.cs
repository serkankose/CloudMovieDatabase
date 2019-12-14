using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudMovieDatabase.Models
{
    public class Actor
    {
        public Actor()
        {
        }

        public Actor(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public int Id { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public virtual ICollection<ActorMovie> Movies {get; set;}
        
    }
}
