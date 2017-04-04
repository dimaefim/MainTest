using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Старый пароль")]
        [DataType(DataType.Password)]
        public String OldPassword { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Новый пароль")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Пароль должен содержать от 4 до 15 символов")]
        [DataType(DataType.Password)]
        public String NewPassword { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Повторите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public String ConfirmNewPassword { get; set; }
    }
}