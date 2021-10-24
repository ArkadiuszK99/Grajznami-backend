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
using Domain.Enum;
using System;

namespace Infrastructure.Services
{
    public class InviteService : IInviteService
    {
        private readonly DataBaseContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public InviteService(DataBaseContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }


        // Funkcja zapisująca użytkownika do eventu 
        public bool SignCurrentUserToEvent(int eventId) 
        {
            var userToSign = GetCurrentUser();
            var eventToSign = _context.Events.Where(x => x.Id == eventId).Include(x => x.Users).SingleOrDefault();
            if (userToSign == null || eventToSign == null)
                return false;
            eventToSign.Users.Add(userToSign);
            _context.Events.Update(eventToSign);
            userToSign.Events.Add(eventToSign);
            _context.Users.Update(userToSign);
            return _context.SaveChanges() > 0;
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

        public List<InviteListUserDTO> GetUsersToInvite(int eventId)
        {
            var usersToInvite = _context.Users.Include(x => x.Events).ToList();
            usersToInvite.Remove(GetCurrentUser());

            var @event = _context.Events.Where(x => x.Id == eventId).Include(x => x.Users).Include(x => x.InvitedUsers).Include(x => x.Sport).SingleOrDefault();

            foreach (var user in @event.Users)
            {
                usersToInvite.Remove(user);
            }

            foreach (var user in @event.InvitedUsers)
            {
                usersToInvite.Remove(user);
            }

            foreach (var user in usersToInvite)
            {
                if (user.City == @event.Users.First().City)
                {
                    user.InvitePoints += 200;
                }
                if (user.FavouriteSport == @event.Sport.Name)
                {
                   user.InvitePoints += 100;
                }
                if (user.Skill == @event.Users.First().Skill)
                {
                    user.InvitePoints += 50;
                }

                DateTime minusFive = @event.Users.First().DateOfBirth.AddYears(-5);
                DateTime plusFive = @event.Users.First().DateOfBirth.AddYears(5);
                int minusResult = DateTime.Compare(user.DateOfBirth, minusFive);
                int plusResult = DateTime.Compare(user.DateOfBirth, plusFive);

                if (minusResult > 0 && plusResult <0)
                {
                    user.InvitePoints += 30;
                }

                int i = 0;
                foreach (var participatedEvent in user.Events)
                {
                    i++;
                    if(participatedEvent.Sport == @event.Sport)
                    {
                        user.InvitePoints += 20;
                    }
                    if (i == 5) break;
                }
                i = 0;
            }
            var usersToInviteDescending = usersToInvite.OrderByDescending(x => x.InvitePoints).ToList();

            var userList = usersToInviteDescending.Select(item => _mapper.Map<User, InviteListUserDTO>(item)).ToList();
            return userList;
        }

        public async Task<List<ReturnEventDTO>> GetInvitations()
        {
            var user = GetCurrentUser();
            var events = await _context.Events.Where(a => a.InvitedUsers.Contains(user)).Include(x => x.Users).ToListAsync();
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
    }
}
