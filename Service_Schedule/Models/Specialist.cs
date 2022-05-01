using Service_Schedule.Utilits;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Schedule.Models
{
    public class Specialist
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Patronymic { get; set; }

        [Required]
        public bool Accepts { get; set; }

        [Required]
        public string Discription { get; set; }

        [Required]
        public string TypeSpec { get; set; }

        [Required]
        public byte[] Avatar { get; set; }

        [ForeignKey("Id")]
        public User User { get; set; }
        public SpecialistView ConvertToSpecialistView()
        {
            return new SpecialistView()
            {
                Surname = Surname,
                Avatar_byte = Avatar,
                Patronymic = Patronymic,
                Accepts = Accepts,
                Discription = Discription,
                TypeSpec = TypeSpec,
                AccountViewModel = Utilit.ConvertUserByAccountUser(User)
            }; 
        }
    }
}
