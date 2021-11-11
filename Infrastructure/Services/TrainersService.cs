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
    public class TrainersService : ITrainersService
    {
        private readonly DataBaseContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public TrainersService(DataBaseContext context, ICurrentUserService currentUserService, IMapper mapper)
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
    }
}
