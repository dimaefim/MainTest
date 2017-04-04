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
    public class HomeController : Controller
    {
        private UserEntity _currentUser;
        private readonly IUsersRepository _usersRepository = NinjectBindings.Instance.Get<IUsersRepository>();

        public HomeController()
        {
            ViewBag.RenderMenu = true;
            SetCurrentUser();
        }

        private void SetCurrentUser()
        {
            _currentUser = SessionCache.CurrentUser;
        }

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

        public ActionResult ShowAllUsers()
        {
            return View();
        }

        public JsonResult GetAllUsers()
        {
            return Json(_usersRepository.GetAllUsers(_currentUser.Id));
        }

        public JsonResult GetMyFriends()
        {
            return Json(_usersRepository.GetMyFriends(_currentUser.Id));
        }

        public JsonResult GetRequests()
        {
            return Json(_usersRepository.GetRequests(_currentUser.Id));
        }

        public JsonResult GetMyRequests()
        {
            return Json(_usersRepository.GetMyRequests(_currentUser.Id));
        }

        public JsonResult AddRequestToFriendList(int id)
        {
            var obj = new {response = _usersRepository.AddRequestToFriendList(_currentUser.Id, id)};

            return Json(obj);
        }

        public ActionResult ShowMyFriends()
        {
            return View();
        }

        public ActionResult ShowUserPage(int id)
        {
            var model = _usersRepository.GetUserPage(_currentUser, id);

            ViewBag.Title = model.Name + " " + model.Surname;

            return View(model);
        }

        public ActionResult AddRequestToFriendListFromUserPage(int id = 0)
        {
            if (_usersRepository.AddRequestToFriendList(_currentUser.Id, id).Equals("false"))
            {
                return RedirectToAction("ErrorCode500", "Error");
            }

            return RedirectToAction("ShowUserPage", "Home", new {id = id});
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

        public ActionResult MyDialogs()
        {
            return View();
        }

        public JsonResult GetAllDialogs()
        {
            return Json(_usersRepository.GetAllDialogs(_currentUser.Id));
        }

        public ActionResult OpenDialog(int id = 0)
        {
            if (!_usersRepository.CheckExistenceDialog(_currentUser.Id, id) &&
                !_usersRepository.CreateNewDialog(new[] {_currentUser.Id, id}))
            {
                return RedirectToAction("ErrorCode500", "Error");
            }

            var dialogId = _usersRepository.GetDialogId(new[] { _currentUser.Id, id });

            if(dialogId == -1)
            {
                return RedirectToAction("ErrorCode500", "Error");
            }

            ViewBag.DialogId = dialogId;

            return View();
        }

        public ActionResult OpenDialogByDialogId(int id = 0)
        {
            ViewBag.DialogId = id;

            return View();
        }

        public JsonResult GetMessages(int id)
        {
            return Json(_usersRepository.GetMessages(id));
        }

        [HttpPost]
        public JsonResult SendMessage(int id, string message)
        {
            return Json(_usersRepository.SendMessage(id, message, _currentUser.Id)
                ? new {response = "true"}
                : new {response = "false"});
        }
    }
}