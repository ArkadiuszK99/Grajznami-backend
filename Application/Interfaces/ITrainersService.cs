using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.DTOs.EventDTOs;
using Application.DTOs.UserDTOs;

namespace Application.Interfaces
{
    public interface ITrainersService
    {
        public Task<List<TrainersDTO>> GetTrainers(string sportName);
        public Task<List<ReturnEventDTO>> GetTrainedEvents();
        public Task<bool> IsTrainer();
        public User GetCurrentUser();
    }
}
