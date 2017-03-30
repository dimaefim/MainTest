using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Windows.Forms;
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
                AboutMe = _currentUser.Settings.aboutMe,
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
                DateOfBirth = _currentUser.DateOfBirth,
                AboutMe = _currentUser.Settings.aboutMe
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
            if (uploadImage != null)
            {
                if (_usersRepository.SaveNewCurrentUserMainPhoto(uploadImage, _currentUser))
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(uploadImage);
        }

        public ActionResult ShowAllUsers()
        {
            return View();
        }

        public JsonResult GetAllUsers()
        {
            return Json(_usersRepository.GetAllUsers(_currentUser));
        }

        public JsonResult GetMyFriends()
        {
            return Json(_usersRepository.GetMyFriends(_currentUser));
        }

        public JsonResult GetRequests()
        {
            return Json(_usersRepository.GetRequests(_currentUser));
        }

        public JsonResult GetMyRequests()
        {
            return Json(_usersRepository.GetMyRequests(_currentUser));
        }

        public JsonResult AddRequestToFriendList(int id)
        {
            var obj = new {response = _usersRepository.AddRequestToFriendList(_currentUser, id)};

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
            if (_usersRepository.AddRequestToFriendList(_currentUser, id).Equals("false"))
            {
                RedirectToAction("ErrorCode403", "Error");
            }

            return RedirectToAction("ShowUserPage", "Home", new {id = id});
        }
    }
}