using Microsoft.AspNetCore.Mvc;
namespace ProjBlog.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(T result, string notFoundMessage = "Not found")
        {
            if (result == null)
                return NotFound(notFoundMessage);

            return Ok(result);
        }

        protected IActionResult HandleCreatedResult(string nameMethod,int id, object? value = null)
        {
            return CreatedAtAction(nameMethod ,new { id }, value);
        }
    }
}
