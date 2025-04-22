using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheMovie.Models;
using TheMovie.Services;

namespace TheMovie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        [Authorize] // Ensure only logged in users can post
        public async Task<IActionResult> PostComment([FromBody] CreateComment dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            if (userId == null)
                return Unauthorized();

            await _commentService.AddCommentAsync(userId, dto);
            return Ok();
        }

        [HttpGet("{movieId}")]
        public async Task<ActionResult<List<Comment>>> GetComments(int movieId)
        {
            var comments = await _commentService.GetCommentsForMovieAsync(movieId);
            return Ok(comments);
        }
    }
}
