using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjBlog.Controllers;
using ProjBlog.Controllers.RequestForm;
using ProjBlogDb.Models;
using ProjBlog.Repository;
namespace ProjBlogTest.UnitTests
{

    public class ArticlesControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ArticlesController>> _loggerMock;
        private readonly ArticlesController _controller;

        public ArticlesControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ArticlesController>>();
            _controller = new ArticlesController(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllArticles_ReturnsOkWithPublishedArticles()
        {
            // Arrange
            var articles = new List<Article>
        {
            new Article { Id = 1, Title = "Test 1", IsPublished = true },
            new Article { Id = 2, Title = "Test 2", IsPublished = true }
        };
            _unitOfWorkMock.Setup(u => u.Articles.GetPublishedArticlesAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(articles);

            // Act
            var result = await _controller.GetAllArticles(publishedOnly: true);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(articles, okResult.Value);
        }

        [Fact]
        public async Task CreateArticle_ReturnsNoContent_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            var request = new CreateArticleRequest
            {
                Title = null!,
                Content = null!,
                UserId = 0,
                Tags = new List<string>()
            };

            // Act
            var result = await _controller.CreateArticle(request);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task EditArticle_ReturnsBadRequest_WhenUserNotFound()
        {
            // Arrange
            var request = new EditingArticleRequest
            {
                UserId = 999,
                ArticleId = 1,
                Title = "New Title",
                Content = "New Content",
                IsPublished = false
            };
            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(request.UserId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((User)null!);

            // Act
            var result = await _controller.EditArticle(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User not found", badRequestResult.Value);
        }


        [Fact]
        public async Task PublishArticle_ReturnsNotFound_WhenArticleDoesNotExist()
        {
            // Arrange
            int articleId = 999;
            _unitOfWorkMock.Setup(u => u.Articles.GetByIdAsync(articleId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((Article)null!);

            // Act
            var result = await _controller.PublishArticle(articleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Article with ID {articleId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteArticle_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int articleId = 1;
            var article = new Article { Id = articleId };
            _unitOfWorkMock.Setup(u => u.Articles.GetByIdAsync(articleId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(article);

            // Act
            var result = await _controller.DeleteArticle(articleId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task GetByAuthor_ReturnsEmptyList_WhenNoArticlesFound()
        {
            // Arrange
            int authorId = 1;
            _unitOfWorkMock.Setup(u => u.Articles.GetByAuthorIdAsync(authorId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new List<Article>());

            // Act
            var result = await _controller.GetByAuthor(authorId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty((IEnumerable<Article>)okResult.Value!);
        }
    }
}


