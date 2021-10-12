using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TokenRefresher : ITokenRefresher
    {
        private readonly byte[] _key;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly DataBaseContext _context;

        public TokenRefresher(IJwtAuthenticationManager jwtAuthenticationManager, DataBaseContext dataBaseContext, IConfiguration config)
        {
            _context = dataBaseContext;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _key = Encoding.ASCII.GetBytes(config["JwtSettings:JwtKey"]);
        }

        public JwtResponse Refresh(RefreshCred refreshCred)
        {
            var tokenHander = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            var principle = tokenHander.ValidateToken(refreshCred.JwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                }, out validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token passed2!");
            }

            var userEmail = principle.Identity.Name;

            var user = _context.Users.SingleOrDefault(x => x.Email == userEmail);
            if (refreshCred.RefreshToken != user.RefreshToken)
            {
                throw new SecurityTokenException("Invalid token passed!");
            }

            return _jwtAuthenticationManager.Authenticate(userEmail, principle.Claims.ToArray());
        }
    }
}
