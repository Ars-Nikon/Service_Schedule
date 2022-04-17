using Microsoft.AspNetCore.Identity;
using System;

namespace Service_Schedule.Models
{
    public class User : IdentityUser
    {
        public bool? Gender { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
