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

        private readonly IConfiguration _iconfiguration;
        public AuthApplication(IConfiguration config, IHttpContextAccessor httpContextAccessor) {
            _config = config;       
            _httpContextAccessor = httpContextAccessor;
        }
        
        public LoggedUser GetLoggedUser()
        {
            LoggedUser result = new LoggedUser()
            {
                Authenticated = false
            };
            try
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
                {
                    string beraerToken = GetBearerToken();
                    if (((TokenHandler)new JwtSecurityTokenHandler()).ReadToken(beraerToken) is JwtSecurityToken jwtSecurityToken)
                    {
                        Claim claim = jwtSecurityToken.Claims.FirstOrDefault((Claim n) => n.Type.Equals("name"));
                        if (claim != null) result = JsonSerializer.Deserialize<LoggedUser>(claim.Value);
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
            text = ((text == null) ? "" : text);
            return text.Replace("Bearer", "").Replace(" ", "").Replace("bearer", "")
                .Replace(" ", "");
        }
    }
}
