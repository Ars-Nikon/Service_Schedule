using System;
using System.ComponentModel.DataAnnotations;

namespace Service_Schedule.Models
{
    public class RegisterViewModel
    {
        public string ReturnUrl { get; set; }

        [EmailAddress(ErrorMessage ="Неверный формат почты")]
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
        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "*Не указан пароль")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        [Required(ErrorMessage = "*Не указан пароль")]
        public string PasswordConfirm { get; set; }
    }
}

