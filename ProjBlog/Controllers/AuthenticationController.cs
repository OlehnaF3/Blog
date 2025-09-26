using clickertime.Hashers;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ProjBlog.Repository;
using ProjBlog.Services;
using System.Security.Claims;

namespace ProjBlog.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthenticationController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IJwtService _jwtService;

        public AuthenticationController(IUnitOfWork unitOfWork, ILogger<AuthenticationController> logger, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtService = jwtService;
        }

        //POST: api/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest("Request was null");

            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null)
                    return NotFound("User not found");

                var roleUser = await _unitOfWork.RoleUser.GetByIdAsync(user.RoleUserId);
                if (roleUser == null)
                    return NotFound("RoleUser not found");

                var role = await _unitOfWork.Role.GetByIdAsync(roleUser.RolesId);
                _logger.LogInformation($"{user.Id},\n{user.Username},\n{user.Email},\n{user.RoleUserId},\n{roleUser.UserId},\n{roleUser.RolesId},\n{role.Name}");
                if (role == null)
                    return NotFound("Role not found");

                if (Hasher.Verify(request.Password, user.PasswordHash))
                {
                    var claim = new List<Claim> //Пока такой набор клаймов
                    {
                        new Claim(ClaimTypes.Name, request.Email),
                        new Claim(ClaimTypes.Role,role.Name)
                    };

                    var jwt = _jwtService.GenerateToken(claim);


                    return Ok(new { Key = jwt });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
            return BadRequest("Request is bad");

        }
    }
}
