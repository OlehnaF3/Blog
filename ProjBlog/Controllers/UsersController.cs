using clickertime.Hashers;
using Microsoft.AspNetCore.Mvc;
using ProjBlogDb.Models;
using ProjBlog.Repository;

namespace ProjBlog.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUnitOfWork unitOfWork, ILogger<UsersController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
                return HandleResult(user, $"User with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Проверка уникальности
                if (await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken))
                    return Conflict("Username already exists");

                if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
                    return Conflict("Email already exists");
                    await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = Hasher.Hash(request.Password),
                    IsActive = true,
                };
                var prot = await _unitOfWork.Users.AddAsync(user, cancellationToken) ?? throw new Exception("User not found");

                await _unitOfWork.SaveChangesAsync(cancellationToken);


                var roles = new RoleUser()
                {
                    RolesId = 3,
                    UserId = prot.Entity.Id,
                };
                await _unitOfWork.RoleUser.AddAsync(roles);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                prot.Entity.RoleUserId = roles.Id;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);


                return HandleCreatedResult(nameof(CreateUser), user.Id, user);
            }
            catch (Exception ex)
            {
                await  _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] EditingUserRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
                if (user == null)
                    return NotFound($"User with ID {id} not found");

                // Проверка уникальности при изменении
                if (!string.IsNullOrEmpty(request.Username) &&
                    request.Username != user.Username &&
                    await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken))
                    return Conflict("Username already exists");

                if (!string.IsNullOrEmpty(request.Email) &&
                    request.Email != user.Email &&
                    await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
                    return Conflict("Email already exists");

                // Обновление полей
                if (!string.IsNullOrEmpty(request.Username))
                    user.Username = request.Username;

                if (!string.IsNullOrEmpty(request.Email))
                    user.Email = request.Email;

                if (request.IsActive)
                    user.IsActive = request.IsActive;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
                if (user == null)
                    return NotFound($"User with ID {id} not found");

                _unitOfWork.Users.Remove(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/users/username/admin
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByUsernameAsync(username, cancellationToken);
                return HandleResult(user, $"User with username '{username}' not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with username {Username}", username);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
