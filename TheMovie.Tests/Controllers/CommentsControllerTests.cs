using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TheMovie.Controllers;
using TheMovie.Models;
using TheMovie.Services;

namespace TheMovie.Tests.Controllers
{
    [TestFixture]
    public class CommentsControllerTests
    {
        private Mock<ICommentService> _commentServiceMock;
        private CommentsController _commentsController;
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _commentServiceMock = new Mock<ICommentService>();

            // Create the controller with a mocked service
            _commentsController = new CommentsController(_commentServiceMock.Object);
        }

        [Test]
        public async Task PostComment_ShouldReturnOk_WhenValidUserPostsComment()
        {
            // Arrange
            var userId = "user1";
            var dto = _fixture.Create<CreateComment>(); 

            // Simulate the user being logged in
            _commentsController.ControllerContext.HttpContext = new DefaultHttpContext();
            _commentsController.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            // Act
            var result = await _commentsController.PostComment(dto);

            // Assert
            result.Should().BeOfType<OkResult>();
            _commentServiceMock.Verify(service => service.AddCommentAsync(userId, dto), Times.Once); 
        }

        [Test]
        public async Task PostComment_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
        {
            // Arrange
            var dto = _fixture.Create<CreateComment>(); 

            // Simulate the user not being logged in (no user in HttpContext)
            _commentsController.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _commentsController.PostComment(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>(); 
        }

        [Test]
        public async Task GetComments_ShouldReturnComments_WhenCommentsExistForMovie()
        {
            // Arrange
            var movieId = 1;
            var comments = _fixture.CreateMany<Comment>(3).ToList(); 

            _commentServiceMock.Setup(service => service.GetCommentsForMovieAsync(movieId))
                               .ReturnsAsync(comments);

            // Act
            var result = await _commentsController.GetComments(movieId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>(); 
            var okResult = result.Result as OkObjectResult; 
            okResult?.Value.Should().BeEquivalentTo(comments); 
            _commentServiceMock.Verify(service => service.GetCommentsForMovieAsync(movieId), Times.Once); 
        }

        public async Task GetComments_ShouldReturnEmptyList_WhenNoCommentsExistForMovie()
        {
            // Arrange
            var movieId = 1;
            var comments = new List<Comment>(); // No comments

            _commentServiceMock.Setup(service => service.GetCommentsForMovieAsync(movieId))
                               .ReturnsAsync(comments);

            // Act
            var result = await _commentsController.GetComments(movieId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var value = okResult?.Value as List<Comment>; 

            value.Should().BeEmpty(); 
            _commentServiceMock.Verify(service => service.GetCommentsForMovieAsync(movieId), Times.Once);
        }
    }
}