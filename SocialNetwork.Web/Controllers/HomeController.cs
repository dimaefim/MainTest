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
            if (_currentUser == null)
            {
                FormsAuthentication.SignOut();
                RedirectToAction("Login", "Account");
            }

            var user = new MainPageViewModel
            {
                Name = _currentUser.Name,
                Surname = _currentUser.Surname,
                DateOfBirth = _currentUser.DateOfBirth,
                AboutMe = _currentUser.Settings.AboutMe,
                MainPhoto = _usersRepository.GetUserMainPhoto(_currentUser.Login)
            };
            
            ViewBag.Title = _currentUser.Name + " " + _currentUser.Surname;

            return View(user);
        }

        public ActionResult EditUserData()
        {
            var updeteUser = new EditProfileViewModel
            {
                Login = _currentUser.Login,
                Name = _currentUser.Name,
                Surname = _currentUser.Surname,
                Patronymic = _currentUser.Patronymic,
                Email = _currentUser.Email,
                DateOfBirth = _currentUser.DateOfBirth.ToString("dd.MM.yyyy"),
                AboutMe = _currentUser.Settings.AboutMe
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
            if (uploadImage != null && _usersRepository.SaveNewCurrentUserMainPhoto(uploadImage, _currentUser))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(uploadImage);
        }

        public ActionResult ShowUserPage(int id)
        {
            var model = _usersRepository.GetUserPage(_currentUser, id);

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
                if (!_currentUser.Password.Equals(model.OldPassword))
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