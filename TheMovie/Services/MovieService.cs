using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using TheMovie.Models;

namespace TheMovie.Services
{
    public class MovieService:IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public MovieService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config["TMDB:BearerToken"]);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        }


        public async Task<MovieResponse> GetLatestMoviesAsync(int page = 1)
        {
            var response = await _httpClient.GetAsync($"movie/popular?language=en-US&page={page}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var movieResponse = JsonSerializer.Deserialize<MovieResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return movieResponse ?? new MovieResponse();
        }

        public async Task<MovieResponse> SearchMoviesAsync(string title = null, int? genreId = null, int page = 1)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["page"] = page.ToString(),
                ["include_adult"] = "false",
                ["language"] = "en-US"
            };

            if (!string.IsNullOrWhiteSpace(title))
            {
                queryParams["query"] = Uri.EscapeDataString(title);
            }

            if (genreId != null)
            {
                queryParams["with_genres"] = genreId.Value.ToString();
            }

            // Choose the appropriate endpoint
            string endpoint = !string.IsNullOrWhiteSpace(title) ? "search/movie" : "discover/movie";

            // Add sort parameter if not searching by title
            if (string.IsNullOrWhiteSpace(title))
            {
                queryParams["sort_by"] = "popularity.desc";
            }

            // Build query string
            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            endpoint = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MovieResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new MovieResponse();
        }

        public async Task<MovieDetail> GetMovieDetailWithExtrasAsync(int movieId)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // 1. Fetch movie detail
            var detailResponse = await _httpClient.GetAsync($"movie/{movieId}?language=en-US");
            detailResponse.EnsureSuccessStatusCode();
            var detailJson = await detailResponse.Content.ReadAsStringAsync();
            var detail = JsonSerializer.Deserialize<MovieDetail>(detailJson, options);

            // 2. Fetch credits (cast)
            var creditsResponse = await _httpClient.GetAsync($"movie/{movieId}/credits");
            creditsResponse.EnsureSuccessStatusCode();
            var creditsJson = await creditsResponse.Content.ReadAsStringAsync();
            var creditsDoc = JsonDocument.Parse(creditsJson);
            detail.Cast = creditsDoc.RootElement.GetProperty("cast")
                .EnumerateArray()
                .Take(10)
                .Select(actor => new Actor
                {
                    Id = actor.GetProperty("id").GetInt32(),
                    Name = actor.GetProperty("name").GetString(),
                    Character = actor.GetProperty("character").GetString(),
                    ProfilePath = actor.GetProperty("profile_path").GetString()
                }).ToList();

            // 3. Fetch images
            var imagesResponse = await _httpClient.GetAsync($"movie/{movieId}/images");
            imagesResponse.EnsureSuccessStatusCode();
            var imagesJson = await imagesResponse.Content.ReadAsStringAsync();
            var imagesDoc = JsonDocument.Parse(imagesJson);
            detail.ImageGallery = imagesDoc.RootElement.GetProperty("backdrops")
                .EnumerateArray()
                .Take(10)
                .Select(img => img.GetProperty("file_path").GetString())
                .Where(path => !string.IsNullOrEmpty(path))
                .Select(path => $"https://image.tmdb.org/t/p/w780{path}")
                .ToList();

            return detail!;
        }

        public async Task<MovieResponse> GetTopRatedMovies(int page = 1)
        {
            var response = await _httpClient.GetAsync($"movie/top_rated?language=en-US&page={page}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var movieResponse = JsonSerializer.Deserialize<MovieResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return movieResponse ?? new MovieResponse();
        }
    }
}
