using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Services.Validations;
using AutoMapper;

namespace Infrastructure.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;


        public LoggingService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<bool> Login(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user != null)
            {
                var signInResult = await _userManager.CheckPasswordAsync(user, model.Password);
                return signInResult;
            }
            return false;
        }

        public async Task<bool> Register(RegisterDTO model)
        {
            var user = new User
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description,
                DateOfBirth = model.DateOfBirth
            };
           
            var createAccountResult = await _userManager.CreateAsync(user, model.Password);
            if (createAccountResult.Succeeded)
                return true;
            return false;
        }
    }
}
