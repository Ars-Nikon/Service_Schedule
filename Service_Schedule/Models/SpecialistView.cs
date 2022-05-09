using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Service_Schedule.Models
{
    public class SpecialistView
    {
        [Required(ErrorMessage = "*Фамилия Отсутствует")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "*Отчество Отсутствует")]
        public string Patronymic { get; set; }

        public bool Accepts { get; set; }

        [Required(ErrorMessage = "*Описание Отсутствует")]
        public string Discription { get; set; }

        public IFormFile Avatar { get; set; }

        [Required(ErrorMessage = "*Цена Отсутствует")]
        public float? Price { get; set; }
        public byte[] Avatar_byte { get; set; }

        [Required(ErrorMessage = "*Тип специальности Отсутствует")]
        public string TypeSpec { get; set; }

        public AccountViewModel AccountViewModel { get; set; }

        public bool IsChange(Specialist spec)
        {
            return !(spec.Surname.Equals(Surname?.Trim()) &&
                    spec.Patronymic.Equals(Patronymic?.Trim()) &&
                    spec.Accepts.Equals(Accepts) &&
                    spec.Discription.Equals(Discription?.Trim()) &&
                    spec.TypeSpec.Equals(TypeSpec) &&
                    spec.Price.Equals(Price));
        }
    }
}
