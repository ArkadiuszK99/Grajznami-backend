using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISportService
    {
        public Task<List<Sport>> GetSportTypes();
        public Task<bool> AddSport(string sportName);
        public bool DeleteSport(int id);
        public Task<bool> SaveChangesAsync();
    }
}
