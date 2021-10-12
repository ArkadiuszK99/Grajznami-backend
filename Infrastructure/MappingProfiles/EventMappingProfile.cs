using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.DTOs;
using Application.DTOs.EventDTOs;
using Domain.Entities;

namespace Infrastructure.MappingProfiles
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            CreateMap<AddEventDTO, Event>();
            CreateMap<ModifyEventDTO, Event>();
            CreateMap<Event, ReturnEventDTO>();
            CreateMap<Event, ReturnEventByIdDTO>();
            CreateMap<Event, UserEventsDTO>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.SportName, o => o.MapFrom(s => s.SportId.ToString()))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.Place, o => o.MapFrom(s => s.Place))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date));
            CreateMap<Event, EventUserNameDTO>();
            CreateMap<User, UserEventsDTO>();
        }
    }
}
