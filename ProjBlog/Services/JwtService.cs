using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjBlog.Services
{
    public class JwtService : IJwtService
    {
        private readonly ILogger<JwtService> _logger;
        private readonly IConfiguration _configuration;
        public JwtService(ILogger<JwtService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var secretKey = _configuration["KeySettings:Key"];
            var issuer = _configuration["KeySettings:ISSUER"];
            var audience = _configuration["KeySettings:AUDIENCE"];
            var expiryMinutes = Double.Parse(_configuration["KeySettings:Time"]!);


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? throw new Exception("Secretkey is null,check app.json")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token) // Генерация jwt
                ?? throw new Exception("Error generate jwt");

            try
            {
                ValidateToken(jwt); // Валидация jwt 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in jwt validate");
            }
            return jwt;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["KeySettings:ISSUER"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["KeySettings:AUDIENCE"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["KeySettings:Key"]!)),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null!;
            }
        }
    }
}
