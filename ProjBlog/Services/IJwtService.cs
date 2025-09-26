using System.Security.Claims;

namespace ProjBlog.Services
{

    public interface IJwtService
    {   
        /// <summary>
        /// Метод генерации токена
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        string GenerateToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Метод валидации токена
        /// Используем метод для собственной безопастности
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ClaimsPrincipal ValidateToken(string token);
    }
}
