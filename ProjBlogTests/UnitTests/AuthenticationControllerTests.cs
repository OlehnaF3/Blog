using clickertime.Hashers;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjBlog.Controllers;
using ProjBlogDb.Models;
using ProjBlog.Repository;
using ProjBlog.Services;
using System.Security.Claims;
namespace ProjBlogTest.UnitTests
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AuthenticationController>> _loggerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AuthenticationController>>();
            _jwtServiceMock = new Mock<IJwtService>();
            _controller = new AuthenticationController(_unitOfWorkMock.Object, _loggerMock.Object, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsOkWithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequest { Email = "user@example.com", Password = "Password123" };
            var user = new User
            {
                Email = request.Email,
                PasswordHash = Hasher.Hash(request.Password),
                RoleUserId = 1
            };
            var roleUser = new RoleUser { RolesId = 1 };
            var role = new Role { Name = "Admin" };

            _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.RoleUser.GetByIdAsync(user.RoleUserId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(roleUser);
            _unitOfWorkMock.Setup(u => u.Role.GetByIdAsync(roleUser.RolesId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(role);
            _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<List<Claim>>()))
                          .Returns("dummy-jwt-token");

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("{ Key = dummy-jwt-token }", okResult.Value?.ToString()?.Replace('"', ' '));
        }

        [Fact]
        public async Task Login_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginRequest { Email = "nonexistent@example.com", Password = "Password123" };
            _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((User)null!);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var request = new LoginRequest { Email = "user@example.com", Password = "Password123" };
            var user = new User
            {
                Email = request.Email,
                PasswordHash = Hasher.Hash(request.Password),
                RoleUserId = 9999
            };
            var roleUser = new RoleUser { RolesId = 1 };
            _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.RoleUser.GetByIdAsync(user.RoleUserId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(roleUser);
            _unitOfWorkMock.Setup(u => u.Role.GetByIdAsync(roleUser.RolesId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((Role)null!);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Role not found", notFoundResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenPasswordIsInvalid()
        {
            // Arrange
            var request = new LoginRequest { Email = "user@example.com", Password = "WrongPassword" };
            var user = new User
            {
                Email = request.Email,
                PasswordHash = Hasher.Hash("CorrectPassword"),
                RoleUserId = 1
            };
            var roleUser = new RoleUser { RolesId = 1 };
            var role = new Role { Name = "Admin" };

            _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.RoleUser.GetByIdAsync(user.RoleUserId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(roleUser);
            _unitOfWorkMock.Setup(u => u.Role.GetByIdAsync(roleUser.RolesId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(role);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Request is bad", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsStatusCode500_WhenExceptionOccurs()
        {
            // Arrange
            var request = new LoginRequest { Email = "user@example.com", Password = "Password123" };
            var user = new User
            {
                Email = request.Email,
                PasswordHash = Hasher.Hash(request.Password),
                RoleUserId = 1
            };
            var exception = new Exception("Test exception");
            _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                          .ThrowsAsync(exception);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal(exception.Message, statusCodeResult.Value);
        }
    }
}