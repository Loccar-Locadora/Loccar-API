using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoccarApplication.Interfaces;
using LoccarDomain.LoggedUser.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LoccarApplication
{
    public class AuthApplication : IAuthApplication
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthApplication(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public LoggedUser GetLoggedUser()
        {
            LoggedUser result = new LoggedUser()
            {
                Authenticated = false,
            };
            try
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
                {
                    string bearerToken = GetBearerToken();
                    if (!string.IsNullOrEmpty(bearerToken) && 
                        ((TokenHandler)new JwtSecurityTokenHandler()).ReadToken(bearerToken) is JwtSecurityToken jwtSecurityToken)
                    {
                        // Extract name from claims
                        Claim nameClaim = jwtSecurityToken.Claims.FirstOrDefault(c => 
                            c.Type.Equals("name", StringComparison.OrdinalIgnoreCase) || 
                            c.Type.Equals(ClaimTypes.Name, StringComparison.OrdinalIgnoreCase));
                        
                        // Extract id from claims
                        Claim idClaim = jwtSecurityToken.Claims.FirstOrDefault(c => 
                            c.Type.Equals("id", StringComparison.OrdinalIgnoreCase) || 
                            c.Type.Equals("sub", StringComparison.OrdinalIgnoreCase) ||
                            c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase));
                        
                        // Extract roles from claims
                        var roleClaims = jwtSecurityToken.Claims.Where(c => 
                            c.Type.Equals("role", StringComparison.OrdinalIgnoreCase) || 
                            c.Type.Equals("roles", StringComparison.OrdinalIgnoreCase) ||
                            c.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase));

                        if (nameClaim != null)
                        {
                            result.name = nameClaim.Value;
                            result.Authenticated = true;
                        }

                        if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
                        {
                            result.id = userId;
                        }

                        if (roleClaims.Any())
                        {
                            result.Roles = roleClaims.Select(r => r.Value).ToList();
                        }
                        else
                        {
                            result.Roles = new List<string>();
                        }
                    }
                }
            }
            catch
            {
            }

            return result;
        }

        private string GetBearerToken()
        {
            string text = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            text = (text == null) ? string.Empty : text;
            return text.Replace("Bearer", string.Empty).Replace(" ", string.Empty).Replace("bearer", string.Empty)
                .Replace(" ", string.Empty);
        }
    }
}
