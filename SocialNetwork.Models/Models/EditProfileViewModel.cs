using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SocialNetwork.Models.Models
{
    public class EditProfileViewModel
    {
        [HiddenInput(DisplayValue = false)]
        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [Display(Name = "Логин")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Логин должен содержать от 4 до 15 символов")]
        public String Login { get; set; }

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
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Это поле не может быть пустым")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Электронная почта")]
        public String Email { get; set; }

        [Display(Name = "О себе")]
        public string AboutMe { get; set; }
    }
}