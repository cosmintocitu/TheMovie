namespace TheMovie.Models
{
    public class MovieResponse
    {
        public int Page { get; set; }
        public List<Movie> Results { get; set; }
        public int Total_Pages { get; set; }
        public int Total_Results { get; set; }
    }
}
