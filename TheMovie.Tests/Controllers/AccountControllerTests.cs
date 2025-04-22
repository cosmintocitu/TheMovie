using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMovie.Controllers;
using TheMovie.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace TheMovie.Tests.Controllers
{
    [TestFixture]
    public class AccountControllerTests
    {
        private IFixture _fixture;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<SignInManager<User>> _signInManagerMock;
        private Mock<IConfiguration> _configMock;
        private AccountController _controller;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            // Setup UserManager
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Setup SignInManager
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null);

            // Setup config
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["Jwt:Key"]).Returns("TestSecretKeyForUnitTests123456!");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _controller = new AccountController(
                _userManagerMock.Object,
                _configMock.Object,
                _signInManagerMock.Object);
        }

        [Test]
        public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var dto = _fixture.Create<AccountController.RegisterDto>();

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(x => x.SignInAsync(It.IsAny<User>(), false, null))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var dto = _fixture.Create<AccountController.RegisterDto>();
            var errors = new[] { new IdentityError { Description = "Email is already taken" } };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(errors);
        }

        [Test]
        public async Task Login_ShouldReturnOkWithToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var dto = _fixture.Build<AccountController.LoginDto>()
                .With(x => x.Password, "Test123!")
                .Create();

            var user = new User { Id = "123", UserName = dto.UserName };

            _userManagerMock.Setup(x => x.FindByNameAsync(dto.UserName)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Match<object>(obj =>
                    obj.GetType().GetProperty("token") != null);
        }

        [Test]
        public async Task Login_ShouldReturnUnauthorized_WhenUserIsNotFound()
        {
            // Arrange
            var dto = _fixture.Create<AccountController.LoginDto>();

            _userManagerMock.Setup(x => x.FindByNameAsync(dto.UserName)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Test]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var dto = _fixture.Create<AccountController.LoginDto>();
            var user = new User { Id = "456", UserName = dto.UserName };

            _userManagerMock.Setup(x => x.FindByNameAsync(dto.UserName)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}


