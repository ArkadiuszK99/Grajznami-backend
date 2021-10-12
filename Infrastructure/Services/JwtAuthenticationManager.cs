using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly string _key;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly DataBaseContext _context;

        public JwtAuthenticationManager(IRefreshTokenGenerator refreshTokenGenerator, DataBaseContext context, IConfiguration config)
        {
            _refreshTokenGenerator = refreshTokenGenerator;
            _context = context;
            _key = config["JwtSettings:JwtKey"];
        }

        public JwtResponse Authenticate(string userEmail, Claim[] claims)
        {
            var token = GenerateTokenString(userEmail, DateTime.UtcNow, claims);
            var refreshToken = _refreshTokenGenerator.GenerateToken();

            var user = _context.Users.SingleOrDefault(x => x.Email == userEmail);
            user.RefreshToken = refreshToken;
            _context.Update(user);
            _context.SaveChanges();

            return new JwtResponse
            {
                JwtToken = token,
                RefreshToken = refreshToken
            };
        }

        public JwtResponse Authenticate(LoginDTO model)
        {
            var token = GenerateTokenString(model.Email, DateTime.UtcNow);
            var refreshToken = _refreshTokenGenerator.GenerateToken();

            var user = _context.Users.SingleOrDefault(x => x.Email == model.Email);
            user.RefreshToken = refreshToken;
            _context.Update(user);
            _context.SaveChanges();

            return new JwtResponse
            {
                JwtToken = token,
                RefreshToken = refreshToken
            };
        }


        string GenerateTokenString(string username, DateTime expires, Claim[] claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                 claims ?? new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                //NotBefore = expires,
                Expires = expires.AddMinutes(200),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));



        }
    }
}
