using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Event> Events { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Description { get; set; }
        public string RefreshToken { get; set; }
    }
}