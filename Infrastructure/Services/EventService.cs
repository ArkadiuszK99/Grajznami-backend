using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Interfaces;
using Persistance.Contexts;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Application.DTO;
using Application.DTOs;
using AutoMapper;
using Application.DTOs.EventDTOs;

namespace Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly DataBaseContext _context;
        private readonly IMapper _mapper;
        private readonly IEventUsersService _eventUsersService;

        public EventService(DataBaseContext context, IMapper mapper, IEventUsersService eventUsersService)
        {
            _context = context;
            _mapper = mapper;
            _eventUsersService = eventUsersService;
        }

        public async Task<List<Sport>> GetSportTypes()
        {
            var sportTypes = await _context.Sports.ToListAsync();
            return sportTypes;
        }

        public async Task<List<ReturnEventDTO>> GetAvailableEvents()
        {
            var availableEvents = await _context.Events.Where(a => a.Status == (int)EventStatus.Available).Include(u => u.Users).ToListAsync();
            List<ReturnEventDTO> eventsToReturn = _mapper.Map<List<Event>, List<ReturnEventDTO>>(availableEvents);

            foreach (var @event in eventsToReturn)
            {
                @event.SportName = _context.Sports.Where(x => x.Id == @event.SportId).SingleOrDefault().Name;
                @event.OrganiserName = _context.Users.Where(x => x.Id == @event.OrganiserId).SingleOrDefault().FirstName;
            }

            for (int i = 0; i < availableEvents.Count(); i++)
            {
                eventsToReturn[i].UsersCount = availableEvents[i].Users.Count();
            }

            return eventsToReturn;
        }

        public async Task<ReturnEventByIdDTO> GetEventById(int id)
        {
            var foundEvent = await _context.Events.Where(x => x.Id == id).Include(u => u.Users).FirstOrDefaultAsync();
            ReturnEventByIdDTO eventToReturn = new ReturnEventByIdDTO();
            eventToReturn = _mapper.Map(foundEvent, eventToReturn);
            eventToReturn.SportName = _context.Sports.Where(x => x.Id == eventToReturn.SportId).SingleOrDefault().Name;
            eventToReturn.OrganiserName = _context.Users.Where(x => x.Id == eventToReturn.OrganiserId).SingleOrDefault().FirstName;

            if (foundEvent == null)
                return null;
            else
                return eventToReturn;
        }

        public async Task<EventUserNameDTO> GetEventWithUsername(int id)
        {
            var foundEvent = await _context.Events.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (foundEvent == null)
                return null;
            else
            {
                EventUserNameDTO eventUserName = new EventUserNameDTO();
                _mapper.Map(foundEvent, eventUserName);
                return eventUserName;
            }
        }

        public async Task<bool> AddEvent(AddEventDTO @event)
        {
            Event eventToAdd = new Event();
            eventToAdd = _mapper.Map(@event, eventToAdd);
            eventToAdd.OrganiserId = _eventUsersService.GetCurrentUser().Id;
            eventToAdd.Trainer = await _context.Users.Where(x => x.Email == @event.TrainerEmail).SingleOrDefaultAsync();
            
            if (eventToAdd == null)
                return false;

            await _context.Events.AddAsync(eventToAdd);
            return true;
        }

        public bool DeleteEvent(int id)
        {
            var eventToDelete = _context.Events.Where(x => x.Id == id).SingleOrDefault();
            if (eventToDelete == null)
                return false;
            else
            {
                _context.Events.Remove(eventToDelete);
                return true;
            }
        }

        public bool ModifyEvent(ModifyEventDTO @event, int id)
        {
            var eventToModify = _context.Events.Where(x => x.Id == id).SingleOrDefault();

            if (eventToModify == null)
                return false;

            eventToModify = _mapper.Map(@event, eventToModify);

            _context.Events.Update(eventToModify);
            return true;

        }

        public Event GetLastEvent (string organiserId)
        {
            var lastEvent = _context.Events.Where(o => o.OrganiserId == organiserId).OrderBy(x => x.Id).Last();
            return lastEvent;
        }

        public async Task<bool> SaveChangesAsync ()
        {
            if(await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
