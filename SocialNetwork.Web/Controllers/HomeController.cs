using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ninject;
using SocialNetwork.Core.Cache;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IUsersRepository _usersRepository = NinjectBindings.Instance.Get<IUsersRepository>();

        public ActionResult Index()
        {
            if (CurrentUser == null)
            {
                FormsAuthentication.SignOut();
                RedirectToAction("Login", "Account");
            }

            var user = new MainPageViewModel
            {
                Name = CurrentUser.Name,
                Surname = CurrentUser.Surname,
                DateOfBirth = CurrentUser.DateOfBirth,
                AboutMe = CurrentUser.Settings.AboutMe,
                MainPhoto = _usersRepository.GetUserMainPhoto(CurrentUser.Login)
            };
            
            ViewBag.Title = CurrentUser.Name + " " + CurrentUser.Surname;

            return View(user);
        }

        public ActionResult EditUserData()
        {
            var updeteUser = new EditProfileViewModel
            {
                Login = CurrentUser.Login,
                Name = CurrentUser.Name,
                Surname = CurrentUser.Surname,
                Patronymic = CurrentUser.Patronymic,
                Email = CurrentUser.Email,
                DateOfBirth = CurrentUser.DateOfBirth.ToString("dd.MM.yyyy"),
                AboutMe = CurrentUser.Settings.AboutMe
            };

            return View(updeteUser);
        }

        [HttpPost]
        public  ActionResult EditUserData(EditProfileViewModel newData)
        {
            ViewBag.Message = "Ошибка в заполнении данных";

            if (ModelState.IsValid)
            {
                if (_usersRepository.CheckExistenceEmail(newData.Email, newData.Login))
                {
                    ViewBag.Message = "Указанный адрес электронной почты уже зарегистрирован в системе";

                    return View(newData);
                }

                if (! _usersRepository.UpdateCurrentUser(newData))
                {
                    ViewBag.Message = "Ошибка редактирования данных";

                    return View(newData);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(newData);
        }
        
        public ActionResult LoadPhoto()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadPhoto(HttpPostedFileBase uploadImage)
        {
            if (uploadImage != null && _usersRepository.SaveNewCurrentUserMainPhoto(uploadImage, CurrentUser))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(uploadImage);
        }

        public ActionResult ShowUserPage(int id)
        {
            var model = _usersRepository.GetUserPage(CurrentUser, id);

            ViewBag.Title = model.Name + " " + model.Surname;

            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!CurrentUser.Password.Equals(model.OldPassword))
                {
                    ViewBag.MessageForPassword = "Неверный текущий пароль";

                    return PartialView(model);
                }

                if (_usersRepository.ChangePassword(model))
                {
                    ViewBag.MessageForPassword = "Пароль успешно изменён";

                    return PartialView();
                }

                ViewBag.MessageForPassword = "Ошибка смены пароля";
            }

            return PartialView(model);
        }
    }
}