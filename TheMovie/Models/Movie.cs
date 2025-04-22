namespace TheMovie.Models
{
    public class Movie
    {
        public bool Adult { get; set; } = true;
        public string Backdrop_Path { get; set; }
        public string BelongsToCollection { get; set; }
        public int Budget { get; set; } = 0;
        public int[] Genre_Ids { get; set; }
        public string Homepage { get; set; }
        public int Id { get; set; } = 0;
        public string ImdbId { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public decimal Popularity { get; set; } = 0;
        public string PosterPath { get; set; }
        public string[] ProductionCompanies { get; set; }
        public string[] ProductionCountries { get; set; }
        public string Release_Date { get; set; }
        public int Revenue { get; set; } = 0;
        public int Runtime { get; set; } = 0;
        public string[] SpokenLanguages { get; set; }
        public string Status { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public bool Video { get; set; } = true;
        public int VoteAverage { get; set; } = 0;
        public int VoteCount { get; set; } = 0;
    }
}
