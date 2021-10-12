using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistance.Contexts;

namespace Infrastructure.Services
{
    public class SportService : ISportService
    {
        private readonly DataBaseContext _context;

        public SportService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<Sport>> GetSportTypes()
        {
            var types = await _context.Sports.ToListAsync();
            return types;
        }

        public async Task<bool> AddSport(string sportName)
        {
            Sport sportToAdd = new Sport() {Name = sportName};

            if (sportToAdd == null)
                return false;

            await _context.Sports.AddAsync(sportToAdd);
            return true;
        }

        public bool DeleteSport(int id)
        {
            var sportToDelete = _context.Sports.Where(x => x.Id == id).SingleOrDefault();
            if (sportToDelete == null)
                return false;
            else
            {
                _context.Sports.Remove(sportToDelete);
                return true;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync() > 0)
                return true;
            return false;
        }
    }
}
