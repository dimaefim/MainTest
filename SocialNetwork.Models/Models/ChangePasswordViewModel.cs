using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Пароль")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Пароль должен содержать от 4 до 15 символов")]
        [DataType(DataType.Password)]
        public String OldPassword { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Пароль")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Пароль должен содержать от 4 до 15 символов")]
        [DataType(DataType.Password)]
        public String NewPassword { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Повторите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public String ConfirmNewPassword { get; set; }
    }
}
