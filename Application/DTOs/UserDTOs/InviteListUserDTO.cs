using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDTOs
{
    public class InviteListUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Skill { get; set; }
        public string City { get; set; }
        public int GamesParticipated{ get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
