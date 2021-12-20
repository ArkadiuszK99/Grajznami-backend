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

            if (@event.Trainer != null)
            {
                usersToInvite.Remove(@event.Trainer);
            }

            List<User> usersToRemove = new List<User>();
            foreach (var user in usersToInvite)
            {
                if (user.FavouriteSport != @event.Sport.Name)
                {
                    usersToRemove.Add(user);
                }
            }

            foreach (var user in usersToRemove)
            {
                usersToInvite.Remove(user);
            }

            foreach (var user in usersToInvite)
            {
                if (user.City == @event.Users.First().City)
                {
                    user.InvitePoints += 500;
                }
                
                if (user.Skill == @event.Users.First().Skill)
                {
                    user.InvitePoints += 100;
                }

                DateTime minusFive = @event.Users.First().DateOfBirth.AddYears(-5);
                DateTime plusFive = @event.Users.First().DateOfBirth.AddYears(5);
                int minusResult = DateTime.Compare(user.DateOfBirth, minusFive);
                int plusResult = DateTime.Compare(user.DateOfBirth, plusFive);

                if (minusResult > 0 && plusResult <0)
                {
                    user.InvitePoints += 100;
                }

                foreach (var participatedEvent in user.Events)
                {

                    DateTime todayDate = DateTime.Today;
                    DateTime monthAgoDate = DateTime.Today.AddMonths(-1);
                    DateTime threeMonthsAgoDate = DateTime.Today.AddMonths(-3);
                    int compareTodayDate = DateTime.Compare(participatedEvent.Date, todayDate);
                    int compareMonthAgoDate = DateTime.Compare(participatedEvent.Date, monthAgoDate);
                    int compareThreeMonthsAgoDate = DateTime.Compare(participatedEvent.Date, threeMonthsAgoDate);

                    if (compareTodayDate < 0 && compareMonthAgoDate > 0)
                    {
                        user.InvitePoints += 50;
                    }

                    if (compareMonthAgoDate < 0 && compareThreeMonthsAgoDate > 0)
                    {
                        user.InvitePoints += 20;
                    }
                }
            }
            var usersToInviteDescending = usersToInvite.OrderByDescending(x => x.InvitePoints).ToList();

            var userList = usersToInviteDescending.Select(item => _mapper.Map<User, InviteListUserDTO>(item)).ToList();

            foreach (var user1 in usersToInvite)
            {
                var participatedEvents = user1.Events.Count();
                foreach (var user2 in userList)
                {
                    if (user1.Email == user2.Email)
                    {
                        user2.GamesParticipated = participatedEvents;
                    }
                }
            }
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
            evToReturn = evToReturn.OrderByDescending(x => x.Date).ToList();

            return evToReturn;
        }
    }
}
