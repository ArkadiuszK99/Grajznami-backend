using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILoggingService
    {
        public Task<bool> Login(LoginDTO model);
        public Task<bool> Register(RegisterDTO model);
    }
}
