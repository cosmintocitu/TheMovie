using TheMovie.Models;

namespace TheMovie.Services
{
    public interface IMovieService
    {
        Task<MovieResponse> GetTopRatedMovies(int page = 1);
        Task<MovieResponse> GetLatestMoviesAsync(int page = 1);
        Task<MovieResponse> SearchMoviesAsync(string title = null, int? genreId = null, int page = 1);
        Task<MovieDetail> GetMovieDetailWithExtrasAsync(int movieId);
    }
}
