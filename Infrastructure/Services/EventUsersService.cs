using Application.Interfaces;
using Persistance.Contexts;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.EventDTOs;
using Application.DTOs.UserDTOs;
using AutoMapper;

namespace Infrastructure.Services
{
    public class EventUsersService : IEventUsersService
    {
        private readonly DataBaseContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public EventUsersService(DataBaseContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }


        // Funkcja zapisująca użytkownika do eventu 
        public bool SignCurrentUserToEvent(int eventId) 
        {
            var userToSign = GetCurrentUser();
            var eventToSign = _context.Events.Where(x => x.Id == eventId).Include(x => x.Users).Include(x => x.InvitedUsers).SingleOrDefault();
            if (userToSign == null || eventToSign == null)
                return false;
            eventToSign.Users.Add(userToSign);
            eventToSign.InvitedUsers.Remove(userToSign);
            _context.Events.Update(eventToSign);
            userToSign.Events.Add(eventToSign);
            _context.Users.Update(userToSign);
            return _context.SaveChanges() > 0;
        }

        // Funkcja zapraszająca użytkownika do eventu 
        public bool InviteCurrentUserToEvent(int eventId, string userEmail)
        {
            var userToInvite = _context.Users.Where(x => x.Email == userEmail).Include(x => x.InvitedToEvents).SingleOrDefault();
            var eventToInvite = _context.Events.Where(x => x.Id == eventId).Include(x => x.InvitedUsers).SingleOrDefault();
            if (userToInvite == null || eventToInvite == null)
                return false;
            eventToInvite.InvitedUsers.Add(userToInvite);
            _context.Events.Update(eventToInvite);
            userToInvite.InvitedToEvents.Add(eventToInvite);
            _context.Users.Update(userToInvite);
            return _context.SaveChanges() > 0;
        }


        // Funkcja zwracająca eventy, na które zapisany jest zalogowany użytkownik

        public async Task<List<ReturnEventDTO>> GetEventsUserIsSignedTo()
        {
            var user = GetCurrentUser();
            var events = user.Events;
            List<ReturnEventDTO> evToReturn = _mapper.Map<List<Event>, List<ReturnEventDTO>>(events);

            foreach (var @event in evToReturn)
            {
                @event.SportName = _context.Sports.Where(x => x.Id == @event.SportId).SingleOrDefault().Name;
                @event.OrganiserName = _context.Users.Where(x => x.Id == @event.OrganiserId).SingleOrDefault().FirstName;
            }

            for (int i = 0; i < events.Count(); i++)
            {
                evToReturn[i].UsersCount = events[i].Users.Count();
            }

            return evToReturn;
        }

        // Funkcja zwracająca eventy zalogowanego użytkownika

        public async Task<List<ReturnEventDTO>> GetUserEvents()
        {
            var user = GetCurrentUser();
            var events = user.Events.Where(x => x.OrganiserId == user.Id).ToList();
            List<ReturnEventDTO> evToReturn = _mapper.Map<List<Event>, List<ReturnEventDTO>>(events);

            foreach (var @event in evToReturn)
            {
                @event.SportName = _context.Sports.Where(x => x.Id == @event.SportId).SingleOrDefault().Name;
                @event.OrganiserName = _context.Users.Where(x => x.Id == @event.OrganiserId).SingleOrDefault().FirstName;
            }

            for (int i = 0; i < events.Count(); i++)
            {
                evToReturn[i].UsersCount = events[i].Users.Count();
            }

            return evToReturn;
        }

        // Funkcja zwracająca eventy, na które zapisany jest zaproszony użytkownik
        public List<UserEventsDTO> GetUserInvitations()
        {
            var user = GetCurrentUser();
            UserEventsDTO userEvents = new UserEventsDTO();
            var eventList = user.InvitedToEvents.Select(x => _mapper.Map(x, userEvents)).ToList();

            return eventList;
        }

        // Funkcja zwracająca imiona i nazwiska osób zapisanych na dane wydarzenie
        public List<EventUsersDTO> GetEventUsers(int eventId)
        {
            var @event = _context.Events.Where(x => x.Id == eventId).Include(x => x.Users).SingleOrDefault();
            EventUsersDTO eventUsers = new EventUsersDTO();
            var userList = @event.Users.Select(u => _mapper.Map(u, eventUsers)).ToList();

            return userList;
        }

        // Funkcja zwracająca aktualnie zalogowanego użytkownika
        public User GetCurrentUser()
        {
            string email = _currentUserService.GetEmail();
            var user = _context.Users.Where(x => x.Email == email).Include(e => e.Events).SingleOrDefault();
            return user;
        }
        public bool SignOutCurrentUserFromEvent(int eventId)
        {
            var userToSign = GetCurrentUser();
            var eventToSign = _context.Events.Where(x => x.Id == eventId).Include(x => x.Users).SingleOrDefault();
            if (userToSign == null || eventToSign == null)
                return false;
            eventToSign.Users.Remove(userToSign);
            _context.Events.Update(eventToSign);
            userToSign.Events.Remove(eventToSign);
            _context.Users.Update(userToSign);
            return _context.SaveChanges() > 0;
        }
    }
}
