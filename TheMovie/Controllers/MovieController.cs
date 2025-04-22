using Microsoft.AspNetCore.Mvc;
using TheMovie.Services;

namespace TheMovie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies([FromQuery] int page = 1)
        {
            var result = await _movieService.GetTopRatedMovies(page);
            return Ok(result);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestMovies([FromQuery] int page = 1)
        {
            var latestMovie = await _movieService.GetLatestMoviesAsync(page);
            return Ok(latestMovie);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies(
        [FromQuery] string title,
        [FromQuery] int? genreId,
        [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(title) && genreId == null)
            {
                return BadRequest("Title or genre is required.");
            }

            var response = await _movieService.SearchMoviesAsync(title,genreId,page);

            return Ok(response);
        }

        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetMovieFullDetail(int id)
        {
            var movieDetail = await _movieService.GetMovieDetailWithExtrasAsync(id);
            return Ok(movieDetail);
        }
    }
}
