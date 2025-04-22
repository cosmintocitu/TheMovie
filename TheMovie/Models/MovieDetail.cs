namespace TheMovie.Models
{
    public class MovieDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string Release_Date { get; set; }
        public int? Runtime { get; set; }
        public string Poster_Path { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Actor> Cast { get; set; }
        public List<string> ImageGallery { get; set; }
    }
}
