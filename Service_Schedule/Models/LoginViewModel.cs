using System.ComponentModel.DataAnnotations;

namespace Service_Schedule.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "*Не указана почта")]
        [Display(Name = "Почта")]
        [EmailAddress(ErrorMessage = "Неверный формат почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "*Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
