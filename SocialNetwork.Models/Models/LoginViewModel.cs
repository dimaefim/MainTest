using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Логин")]
        public String Login { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public String Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}