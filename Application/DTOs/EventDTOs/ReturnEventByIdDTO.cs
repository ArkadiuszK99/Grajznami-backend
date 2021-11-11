using Application.DTOs.UserDTOs;
using Domain.Entities;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.EventDTOs
{
    public class ReturnEventByIdDTO
    {
        public int Id { get; set; }
        public int SportId { get; set; }
        public string SportName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public string OrganiserId { get; set; }
        public string OrganiserName { get; set; }
        public string OrganiserEmail { get; set; }
        public string OrganiserPhoneNumber { get; set; }
        public List<EventUsersDTO> Users { get; set; }
        public int UsersLimit { get; set; }
        public int UsersCount { get; set; }
        public EventStatus Status { get; set; }
        public bool IsPublic { get; set; }
        public bool IsFree { get; set; }
        public decimal TotalCost { get; set; }
        public string TrainerEmail { get; set; }
    }
}
