namespace TheMovie.Models
{
    public class CreateComment
    {
        public int MovieId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
