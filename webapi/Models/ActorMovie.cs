namespace CloudMovieDatabase.Models
{
    public class ActorMovie
    {
        public int ActorId { get; set; }
        public virtual Actor Actor { get; set; }
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
    }
}