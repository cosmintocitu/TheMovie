using TheMovie.Models;

namespace TheMovie.Services
{
    public interface ICommentService
    {
        Task AddCommentAsync(string userId, CreateComment dto);
        Task<List<Comment>> GetCommentsForMovieAsync(int movieId);
    }
}
