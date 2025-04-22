namespace TheMovie.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int MovieId { get; set; } // Link to TMDB movie
        public string UserId { get; set; } = string.Empty; // From Identity or auth system
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
