using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ModifyEventDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public int UsersLimit { get; set; }
        public bool IsPublic { get; set; }
        public bool IsFree { get; set; }
        public decimal TotalCost { get; set; }
    }
}
