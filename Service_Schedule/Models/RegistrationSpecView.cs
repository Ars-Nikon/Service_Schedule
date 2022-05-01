using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Service_Schedule.Models
{
    public class RegistrationSpecView : Models.RegisterViewModel
    {
        [Required(ErrorMessage = "*Не указана фамилия")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "*Не указано отчество")]
        public string Patronymic { get; set; }

        [Required]
        public bool Accepts { get; set; }

        [Required(ErrorMessage = "*Не указано описание")]
        public string Discription { get; set; }

        [Required(ErrorMessage = "*Загрузите фото")]
        public IFormFile Avatar { get; set; }

        [Required(ErrorMessage = "*Не указан тип специальности")]
        public string TypeSpec { get; set; }


        public Specialist ConvertToSpecialist()
        {
            byte[] result = null;

            using (var binaryReader = new BinaryReader(Avatar.OpenReadStream()))
            {
                result = binaryReader.ReadBytes((int)Avatar.Length);
            }

            return new Specialist
            {
                Surname = Surname,
                Patronymic = Patronymic,
                Accepts = Accepts,
                Discription = Discription,
                TypeSpec = TypeSpec, 
                Avatar = result
            };
        }

        public User ConvertToUser()
        {
            return new  User
            {
                Email = Email?.Trim(),
                UserName = Email?.Trim(),
                BirthDate = BirthDate,
                Name = Name?.Trim(),
                PhoneNumber = Phone?.Trim(),
                Gender = Gender,
                DateCreate = DateTime.UtcNow.AddHours(3)
            };
        }
    }
}
