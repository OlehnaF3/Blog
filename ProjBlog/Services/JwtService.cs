using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjBlog.Services
{
    public class JwtService : IJwtService
    {
        private readonly ILogger<JwtService> _logger;
        public JwtService(ILogger<JwtService> logger)
        {
                _logger = logger;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var secretKey = Configuration.Config?["KeySettings:Key"];
            var issuer = Configuration.Config?["KeySettings:ISSUER"];
            var audience = Configuration.Config?["KeySettings:AUDIENCE"];
            var expiryMinutes = Double.Parse(Configuration.Config?["KeySettings:Time"]!);

                
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey?? throw new Exception("Secretkey is null,check app.json")));
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
                    ValidIssuer = Configuration.Config?["KeySettings:ISSUER"],
                    ValidateAudience = true,
                    ValidAudience = Configuration.Config?["KeySettings:AUDIENCE"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.Config?["KeySettings:Key"]!)),
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
