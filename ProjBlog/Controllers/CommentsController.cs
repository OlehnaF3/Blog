using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjBlog.Controllers.RequestForm;
using ProjBlog.Models;
using ProjBlog.Repository;

namespace ProjBlog.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentsController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(IUnitOfWork unitOfWork, ILogger<CommentsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/comments/article/5
        [HttpGet("article/{articleId}")]
        public async Task<IActionResult> GetByArticle(int articleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var comments = await _unitOfWork.Comments.GetCommentsWithRepliesAsync(articleId, cancellationToken);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for article {ArticleId}", articleId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/comments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var comment = await _unitOfWork.Comments.GetByIdAsync(id, cancellationToken);
                return HandleResult(comment, $"Comment with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment with ID {CommentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        // POST: api/comments
        [HttpPost("edit")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditComment([FromBody] EditingCommentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // В реальном приложении получаем пользователя из контекста аутентификации
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                    return BadRequest("User not found");

                // Проверяем статью
                var article = await _unitOfWork.Articles.GetByIdAsync(request.UserId, cancellationToken);
                if (article == null)
                    return BadRequest("Article not found");

                // Проверяем родительский комментарий, если указан
                if (request.ParentCommentId.HasValue)
                {
                    var parentComment = await _unitOfWork.Comments.GetByIdAsync(
                        request.ParentCommentId.Value, cancellationToken);
                    if (parentComment == null)
                        return BadRequest("Parent comment not found");
                }
                var oldcomment = await _unitOfWork.Comments.GetByIdAsync(request.CommentId, cancellationToken);
                if(oldcomment == null)
                {
                    return NotFound("Old Comment not Found");
                }
 
                    oldcomment.Content = request.Content;
                    oldcomment.ArticleId = article.Id;
                    oldcomment.UserId = user.Id;
                    oldcomment.ParentCommentId = request.ParentCommentId;
                    oldcomment.IsApproved = false;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return HandleCreatedResult(nameof(CreateComment), oldcomment.Id, oldcomment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/comments
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // В реальном приложении получаем пользователя из контекста аутентификации
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                    return BadRequest("User not found");

                // Проверяем статью
                var article = await _unitOfWork.Articles.GetByIdAsync(request.UserId, cancellationToken);
                if (article == null)
                    return BadRequest("Article not found");

                // Проверяем родительский комментарий, если указан
                if (request.ParentCommentId.HasValue)
                {
                    var parentComment = await _unitOfWork.Comments.GetByIdAsync(
                        request.ParentCommentId.Value, cancellationToken);
                    if (parentComment == null)
                        return BadRequest("Parent comment not found");
                }

                var comment = new Comment
                {
                    Content = request.Content,
                    ArticleId = article.Id,
                    UserId = user.Id,
                    ParentCommentId = request.ParentCommentId,
                    IsApproved = false
                };

                await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return HandleCreatedResult(nameof(CreateComment), comment.Id, comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/comments/5/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveComment(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var comment = await _unitOfWork.Comments.GetByIdAsync(id, cancellationToken);
                if (comment == null)
                    return NotFound($"Comment with ID {id} not found");

                comment.Approve();
                _unitOfWork.Comments.Update(comment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving comment with ID {CommentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/comments/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var comments = await _unitOfWork.Comments.GetByUserIdAsync(userId, cancellationToken);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments by user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var comment = await _unitOfWork.Comments.GetByIdAsync(id, cancellationToken);
                if (comment == null)
                    return NotFound($"Comment with ID {id} not found");

                _unitOfWork.Comments.Remove(comment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID {CommentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
