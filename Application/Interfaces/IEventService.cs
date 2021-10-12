using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Application.DTOs;
using Application.DTO;
using Application.DTOs.EventDTOs;

namespace Application.Interfaces
{
    public interface IEventService
    {
        public Task<List<Sport>> GetSportTypes();
        public Task<List<ReturnEventDTO>> GetAvailableEvents();
        public Task<ReturnEventByIdDTO> GetEventById(int id);
        public Task<bool> AddEvent(AddEventDTO @event);
        public bool DeleteEvent(int id);
        public bool ModifyEvent(ModifyEventDTO @event, int id);
        public Task<EventUserNameDTO> GetEventWithUsername(int id);
        public Event GetLastEvent(string organiserId);
        public Task<bool> SaveChangesAsync();
    }
}
