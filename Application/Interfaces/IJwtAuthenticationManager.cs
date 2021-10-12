using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IJwtAuthenticationManager
    {
        JwtResponse Authenticate (LoginDTO model);
        JwtResponse Authenticate(string userEmail, Claim[] claims);
    }
}
