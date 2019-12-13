using System;
using System.ComponentModel.DataAnnotations;

namespace CloudMovieDatabase.Models
{
    public class Actor
    {
        internal Actor()
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
    }
}
