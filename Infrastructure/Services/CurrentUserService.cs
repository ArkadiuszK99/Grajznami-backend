using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;


namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _accessor;
        public CurrentUserService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string UserMail { get; set; }

        public string GetEmail()
        {
            UserMail = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            return UserMail; 
        }
    }
}
