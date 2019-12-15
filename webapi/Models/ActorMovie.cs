using System;
using System.ComponentModel.DataAnnotations;

namespace CloudMovieDatabase.Models
{
    public class ActorMovie
    {
        [Timestamp] public byte[] Timestamp { get; set; }

        public int ActorId { get; set; }
        public virtual Actor Actor { get; set; }
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
    }
}