using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.DTOs.EventDTOs;
using Application.DTOs.UserDTOs;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IStatsService
    {
        public Task<List<TrainersDTO>> GetTrainers(string sportName);
        public Task<List<ReturnEventDTO>> GetTrainedEvents();
        public Task<StatsDTO> GetStats();
        public Task<bool> IsTrainer();
        public User GetCurrentUser();
    }
}
