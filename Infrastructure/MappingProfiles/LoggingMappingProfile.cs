using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.UserDTOs;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.MappingProfiles
{
    public class LoggingMappingProfile : Profile
    {
        public LoggingMappingProfile()
        {
            CreateMap<User, EventUsersDTO>();
        }
    }
}
