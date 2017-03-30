using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Models
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Логин")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Логин должен содержать от 4 до 15 символов")]
        public String Login { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Пароль")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Пароль должен содержать от 4 до 15 символов")]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Повторите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public String ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Имя")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Фамилия")]
        public String Surname { get; set; }

        [Display(Name = "Отчество")]
        public String Patronymic { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime DateOfBirth { get; set; }
        
        public string DateOfBirthString => DateOfBirth.ToString("dd.MM.yyyy");

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Электронная почта")]
        public String Email { get; set; }
    }
}
