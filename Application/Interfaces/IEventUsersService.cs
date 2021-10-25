using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.DTOs.EventDTOs;
using Application.DTOs.UserDTOs;

namespace Application.Interfaces
{
    public interface IEventUsersService
    {
        public Task<List<ReturnEventDTO>> GetEventsUserIsSignedTo();
        public Task<List<ReturnEventDTO>> GetUserEvents();
        public bool SignCurrentUserToEvent(int eventId);
        public bool InviteCurrentUserToEvent(int eventId, string userEmail);
        public List<EventUsersDTO> GetEventUsers(int eventId);
        public User GetCurrentUser();
        bool SignOutCurrentUserFromEvent(int eventId);
    }
}
