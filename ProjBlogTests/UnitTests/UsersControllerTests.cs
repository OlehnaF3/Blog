using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjBlog.Controllers;
using ProjBlogDb.Models;
using ProjBlog.Repository;
namespace ProjBlogTest.UnitTests
{

    public class UsersControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UsersController>> _loggerMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkWithUsersList()
        {
            // Arrange
            var users = new List<User>
        {
            new User { Id = 1, Username = "user1", Email = "user1@example.com" },
            new User { Id = 2, Username = "user2", Email = "user2@example.com" }
        };
            _unitOfWorkMock.Setup(u => u.Users.GetAllAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 999;
            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((User)null!);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal($"User with ID {userId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnsConflict_WhenUsernameExists()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = "existing_user",
                Email = "new@example.com",
                Password = "Password123"
            };
            _unitOfWorkMock.Setup(u => u.Users.UsernameExistsAsync(request.Username, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Username already exists", conflictResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int userId = 1;
            var existingUser = new User
            {
                Id = userId,
                Username = "old_user",
                Email = "old@example.com"
            };
            var request = new EditingUserRequest
            {
                Username = "new_user",
                Email = "new@example.com",
                IsActive = true
            };

            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(existingUser);
            _unitOfWorkMock.Setup(u => u.Users.UsernameExistsAsync(request.Username, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Users.EmailExistsAsync(request.Email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateUser(userId, request);

            // Assert
            var ContentResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ContentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int userId = 1;
            var existingUser = new User { Id = userId };
            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(existingUser);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task GetByUsername_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "nonexistent";
            _unitOfWorkMock.Setup(u => u.Users.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((User)null!);

            // Act
            var result = await _controller.GetByUsername(username);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal($"User with username '{username}' not found", notFoundResult.Value);
        }
    }
}