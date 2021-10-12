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
    public class InviteMappingProfile : Profile
    {
        public InviteMappingProfile()
        {
            CreateMap<User, InviteListUserDTO>();
        }
    }
}
