using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public Sport Sport { get; set; }
        public int SportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public List<User> Users { get; set; }
        public List<User> InvitedUsers { get; set; }
        public List<User> InvitedTrainers { get; set; }
        public User Trainer { get; set; }
        public string OrganiserId { get; set; }
        public int UsersLimit { get; set; }
        public EventStatus Status { get; set; }
        public bool IsPublic { get; set; }
        public bool IsFree { get; set; }
        public decimal TotalCost { get; set; }
    }
}