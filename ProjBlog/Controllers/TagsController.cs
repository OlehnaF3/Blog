using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjBlog.Controllers.RequestForm;
using ProjBlog.Models;
using ProjBlog.Repository;

namespace ProjBlog.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TagsController> _logger;

        public TagsController(IUnitOfWork unitOfWork, ILogger<TagsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<IActionResult> GetAllTags(CancellationToken cancellationToken = default)
        {
            try
            {
                var tags = await _unitOfWork.Tags.GetAllAsync(cancellationToken);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tags");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/tags/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularTags([FromQuery] int count = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var tags = await _unitOfWork.Tags.GetPopularTagsAsync(count, cancellationToken);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular tags");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/tags/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var tag = await _unitOfWork.Tags.GetByIdAsync(id, cancellationToken);
                return HandleResult(tag, $"Tag with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tag with ID {TagId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/tags
        [HttpPost("edit")]
        public async Task<IActionResult> EditTag([FromBody] EditingTagRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Tag name is required");

            try
            {
                if (await _unitOfWork.Tags.NameExistsAsync(request.Name, cancellationToken))
                    return Conflict($"Tag '{request.Name}' already exists");

                var tag = await _unitOfWork.Tags.GetByNameAsync(request.Name, cancellationToken);
                if(tag == null)
                {
                    return NotFound("Tag not Found");
                }
                tag.Name = request.Name;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return HandleCreatedResult(nameof(CreateTag), tag.Id, tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag '{TagName}'", request.Name);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/tags
        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Tag name is required");

            try
            {
                if (await _unitOfWork.Tags.NameExistsAsync(name, cancellationToken))
                    return Conflict($"Tag '{name}' already exists");

                var tag = new Tag { Name = name.Trim() };

                await _unitOfWork.Tags.AddAsync(tag, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return HandleCreatedResult(nameof(CreateTag), tag.Id, tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag '{TagName}'", name);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var tag = await _unitOfWork.Tags.GetByIdAsync(id, cancellationToken);
                if (tag == null)
                    return NotFound($"Tag with ID {id} not found");

                _unitOfWork.Tags.Remove(tag);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag with ID {TagId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        //POST: api/link
        [HttpPost("link")]
        [Authorize]
        public async Task<IActionResult> LinkTag([FromForm] LinkTagRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                return BadRequest("Request is null");
            try
            {
                var tag = _unitOfWork.Tags.FindAsync(t => t.Id == request.TagId, cancellationToken);
                if (tag == null)
                    return NotFound("Tag not found");

                var article = _unitOfWork.Articles.FindAsync(a => a.Id == request.ArticleId, cancellationToken);
                if (article == null)
                    return NotFound("Article not found");

                var xref = new ArticleTag
                {
                    ArticleId = article.Id,
                    TagId = tag.Id,
                };
                await _unitOfWork.ArticleTag.AddAsync(xref, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Ok(new { xref });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Bad request");
                return StatusCode(500,ex.Message);
            }

        }
    }
}
