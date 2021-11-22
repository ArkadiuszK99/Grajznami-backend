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
using Application.DTOs;
using System;

namespace Infrastructure.Services
{
    public class StatsService : IStatsService
    {
        private readonly DataBaseContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public StatsService(DataBaseContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        // Funkcja zwracająca aktualnie zalogowanego użytkownika
        public User GetCurrentUser()
        {
            string email = _currentUserService.GetEmail();
            var user = _context.Users.Where(x => x.Email == email).Include(e => e.Events).SingleOrDefault();
            return user;
        }

        public async Task<List<TrainersDTO>> GetTrainers(string sportName)
        {
            var trainers = _context.Users.Where(x => x.IsTrainer == true).Where(x => x.TrainedSport == sportName).ToList();
            List<TrainersDTO> trainersToReturn = _mapper.Map<List<User>, List<TrainersDTO>>(trainers).ToList();
            return trainersToReturn;
        }

        public async Task<List<ReturnEventDTO>> GetTrainedEvents()
        {
            var user = GetCurrentUser();
            var events = await _context.Events.Where(x => x.Trainer == user).Include(x => x.Users).ToListAsync();
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

        public async Task<bool> IsTrainer()
        {
            var user = GetCurrentUser();
            if (user!=null)
            {
                return user.IsTrainer;
            }
            return false;
        }

        public async Task<StatsDTO> GetStats()
        {
            var user = GetCurrentUser();
            var OrganisedEvents = await _context.Events.Where(x => x.OrganiserId == user.Id).ToListAsync();
            var @events = await _context.Events.Where(x => x.Users.Contains(user)).Include(x => x.Sport).ToListAsync();
            StatsDTO stats = _mapper.Map<User, StatsDTO>(user);

            int age = DateTime.Now.Subtract(user.DateOfBirth).Days;
            stats.Age = age / 365;
            stats.TotalPlayedMatches = user.Events.Count();
            stats.MatchesPlayedInLastMonth = user.Events.Where(x => DateTime.Now.Subtract(x.Date).Days < 30).Count();
            stats.FavouriteSportMatches = events.Where(x => x.Sport.Name == user.FavouriteSport).Count();

            var sports = await _context.Sports.ToListAsync();
            stats.MostMatchesInSport = 0;
            stats.SportWithMostMatches = "";
            var temp = 0;
            foreach (Sport sport in sports)
            {
                foreach (Event @event in user.Events)
                {
                    if(sport == @event.Sport)
                    {
                        temp += 1;
                    }
                }
                
                if(temp > stats.MostMatchesInSport)
                {
                    stats.MostMatchesInSport = temp;
                    stats.SportWithMostMatches = sport.Name;
                }
                temp = 0;
            }

            stats.NumberOfGamesOrganised = 0;
            foreach (Event @event in OrganisedEvents)
            {
                if(@event.OrganiserId == user.Id)
                {
                    stats.NumberOfGamesOrganised += 1;
                }
            }
            return stats;
        }
    }
}
