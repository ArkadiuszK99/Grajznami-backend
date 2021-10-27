﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDTOs
{
    public class TrainersDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public float TrainingPrice { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string License { get; set; }
    }
}
