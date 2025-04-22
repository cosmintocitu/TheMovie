using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMovie.Controllers;
using TheMovie.Models;
using TheMovie.Services;

namespace TheMovie.Tests.Controllers
{
    [TestFixture]
    public class MovieControllerTests
    {
        private IFixture _fixture;
        private Mock<IMovieService> _movieServiceMock;
        private MovieController _controller;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _movieServiceMock = _fixture.Freeze<Mock<IMovieService>>();
            _controller = new MovieController(_movieServiceMock.Object);
        }

        [Test]
        public async Task GetPopularMovies_ShouldReturnOk_WithMovies()
        {
            // Arrange
            var mockData = _fixture.Create<MovieResponse>();
            _movieServiceMock.Setup(x => x.GetTopRatedMovies(It.IsAny<int>()))
                .ReturnsAsync(mockData);
            // Act
            var result = await _controller.GetPopularMovies();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(mockData);
        }

        [Test]
        public async Task GetLatestMovies_ShouldReturnOk_WithMovies()
        {
            // Arrange
            var mockData = _fixture.Create<MovieResponse>();
            _movieServiceMock.Setup(x => x.GetLatestMoviesAsync(It.IsAny<int>()))
                .ReturnsAsync(mockData); // Mock MovieResponse

            // Act
            var result = await _controller.GetLatestMovies();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(mockData);
        }

        [Test]
        public async Task SearchMovies_ShouldReturnOk_WhenTitleOrGenreIsProvided()
        {
            // Arrange
            var title = "Inception";
            int? genreId = null;
            var searchResults = _fixture.Create<MovieResponse>();

            _movieServiceMock.Setup(x => x.SearchMoviesAsync(title, genreId, 1))
                .ReturnsAsync(searchResults); // Mock MovieResponse

            // Act
            var result = await _controller.SearchMovies(title, genreId, 1);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(searchResults);
        }

        [Test]
        public async Task SearchMovies_ShouldReturnBadRequest_WhenTitleAndGenreAreMissing()
        {
            // Arrange
            string title = null;
            int? genreId = null;

            // Act
            var result = await _controller.SearchMovies(title, genreId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Title or genre is required.");
        }

        [Test]
        public async Task GetMovieFullDetail_ShouldReturnOk_WithDetails()
        {
            // Arrange
            var detail = _fixture.Create<MovieDetail>();
            int movieId = 42;

            _movieServiceMock.Setup(x => x.GetMovieDetailWithExtrasAsync(movieId))
                .ReturnsAsync(detail); // Mock MovieDetail

            // Act
            var result = await _controller.GetMovieFullDetail(movieId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(detail);
        }
    }
}

