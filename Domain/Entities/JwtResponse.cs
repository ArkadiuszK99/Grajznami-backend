﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class JwtResponse
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
