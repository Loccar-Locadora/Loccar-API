using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LoccarTests.IntegrationTests
{
    public static class JwtTokenHelper
    {
        private const string JwtKey = "test-key-for-loccar-integration-tests-must-be-long-enough";
        private const string JwtIssuer = "TestIssuer";
        private const string JwtAudience = "TestAudience";

        public static string GenerateToken(string userId = "1", string email = "test@test.com", params string[] roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(JwtKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = JwtIssuer,
                Audience = JwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GenerateAdminToken() => GenerateToken("1", "admin@test.com", "ADMIN");

        public static string GenerateEmployeeToken() => GenerateToken("2", "employee@test.com", "EMPLOYEE");

        public static string GenerateCommonUserToken() => GenerateToken("3", "user@test.com", "COMMON_USER");
    }
}
