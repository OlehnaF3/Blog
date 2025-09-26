using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjBlog.Controllers.RequestForm;
using ProjBlog.Models;
using ProjBlog.Repository;

namespace ProjBlog.Controllers
{
    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(IUnitOfWork unitOfWork, ILogger<ArticlesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/articles
        [HttpGet]
        public async Task<IActionResult> GetAllArticles([FromQuery] bool publishedOnly = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Article> articles;

                if (publishedOnly)
                    articles = await _unitOfWork.Articles.GetPublishedArticlesAsync(cancellationToken);
                else
                    articles = await _unitOfWork.Articles.GetArticlesWithTagsAndAuthorAsync(cancellationToken);

                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting articles");
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/articles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var article = await _unitOfWork.Articles.GetWithDetailsAsync(id, cancellationToken);
                return HandleResult(article, $"Article with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting article with ID {ArticleId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/articles
        [HttpPost("edit")]
        public async Task<IActionResult> EditArticle([FromBody] EditingArticleRequest request, CancellationToken cancellation = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                    return BadRequest("User not found");

                var article = await _unitOfWork.Articles.GetByIdAsync(request.ArticleId);

                if (article == null)
                    return NotFound("Article not found");

                article.Title = request.Title;
                article.Content = request.Content;
                article.IsPublished = request.IsPublished;

                await _unitOfWork.SaveChangesAsync(cancellation);
                return HandleCreatedResult(nameof(EditArticle), request.ArticleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error editing article");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/articles
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (request.Content == null || request.Title == null || request.UserId < 0) // Проверка реквеста 
                return NoContent();
   
            try
            {
                // Получаем автора (в реальном приложении из контекста аутентификации)
                var author = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
                if (author == null)
                    return BadRequest("Author not found");

                var article = new Article
                {
                    Title = request.Title,
                    Content = request.Content,
                    AuthorId = author.Id,
                    IsPublished = request.IsPublished
                };

                if (request.IsPublished)
                    article.Publish();

                // Добавляем теги
                if (request.Tags.Count > 0)
                {
                    foreach (var tagName in request.Tags)
                    {
                        var tag = await _unitOfWork.Tags.GetByNameAsync(tagName, cancellationToken);
                        if (tag == null)
                        {
                            tag = new Tag { Name = tagName };
                            await _unitOfWork.Tags.AddAsync(tag, cancellationToken);
                        }
                        article.AddTag(tag);
                    }
                }

                await _unitOfWork.Articles.AddAsync(article, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return HandleCreatedResult(nameof(CreateArticle), article.Id, article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/articles/5/publish
        [HttpPut("{id}/publish")]
        [Authorize(Policy = "Moderator,Administrator")]
        public async Task<IActionResult> PublishArticle(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var article = await _unitOfWork.Articles.GetByIdAsync(id, cancellationToken);
                if (article == null)
                    return NotFound($"Article with ID {id} not found");

                article.Publish();
                _unitOfWork.Articles.Update(article);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing article with ID {ArticleId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/articles/author/5
        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(int authorId, CancellationToken cancellationToken = default)
        {
            try
            {
                var articles = await _unitOfWork.Articles.GetByAuthorIdAsync(authorId, cancellationToken);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting articles by author {AuthorId}", authorId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/articles/tag/5
        [HttpGet("tag/{tagId}")]
        public async Task<IActionResult> GetByTag(int tagId, CancellationToken cancellationToken = default)
        {
            try
            {
                var articles = await _unitOfWork.Articles.GetByTagIdAsync(tagId, cancellationToken);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting articles by tag {TagId}", tagId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/articles/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "Moderator,Administrator")]
        public async Task<IActionResult> DeleteArticle(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var article = await _unitOfWork.Articles.GetByIdAsync(id, cancellationToken);
                if (article == null)
                    return NotFound($"Article with ID {id} not found");

                _unitOfWork.Articles.Remove(article);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article with ID {ArticleId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
