using System;
using System.ComponentModel.DataAnnotations;

namespace Service_Schedule.Models
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "*Id Отсутствует")]
        public string Id { get; set; }

        [EmailAddress(ErrorMessage = "Неверный формат почты")]
        [Display(Name = "Почта")]
        [Required(ErrorMessage = "*Не указана почта")]
        public string Email { get; set; }

        [Display(Name = "Пол")]
        [Required(ErrorMessage = "*Не указан пол")]
        public bool? Gender { get; set; }

        [Required(ErrorMessage = "*Не указано имя")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "Неверный формат номера")]
        [Display(Name = "Номер Телефона")]
        [Required(ErrorMessage = "*Не указан номер телефона")]
        public string Phone { get; set; }

        [Display(Name = "Дата рождения")]
        [Required(ErrorMessage = "*")]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Сменить пароль")]
        public string Password { get; set; }

        public bool IsChange(User user)
        {
            return !(user.BirthDate.Equals(BirthDate) &&
                user.PhoneNumber.Equals(Phone?.Trim()) &&
                user.UserName.Equals(Email?.Trim()) &&
                user.Email.Equals(Email?.Trim()) &&
                user.Gender.Equals(Gender) &&
                user.Name.Equals(Name?.Trim()));
        }
    }
}
