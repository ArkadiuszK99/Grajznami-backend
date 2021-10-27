using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Event> Events { get; set; }
        public List<Event> InvitedToEvents { get; set; }
        public List<Event> InvitedToTrainEvents { get; set; }
        public List<Event> TrainedEvents { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Description { get; set; }
        public string RefreshToken { get; set; }
        public string FavouriteSport { get; set; }
        public string Skill { get; set; }
        public string City { get; set; }
        public int InvitePoints { get; set; }
        public bool IsTrainer { get; set; }
        public string TrainedSport { get; set; }
        public float TrainingPrice { get; set; }
        public string License { get; set; }
    }
}