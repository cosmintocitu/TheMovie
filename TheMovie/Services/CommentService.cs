using Microsoft.EntityFrameworkCore;
using System;
using TheMovie.Data;
using TheMovie.Models;

namespace TheMovie.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _db;

        public CommentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddCommentAsync(string userId, CreateComment dto)
        {
            var comment = new Comment
            {
                MovieId = dto.MovieId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetCommentsForMovieAsync(int movieId)
        {
            return await _db.Comments
                .Where(c => c.MovieId == movieId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
