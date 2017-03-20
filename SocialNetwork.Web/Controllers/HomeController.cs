using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Windows.Forms;
using SocialNetwork.Core.Cache;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Core.UnitOfWork;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserEntity _currentUser;
        
        public HomeController()
        {
            SetCurrentUser();
        }

        public async void SetCurrentUser()
        {
            _currentUser = await UserData.db.WorkWithUser.GetUserByLoginOrEmail(HttpContext.Cur);
        }
        public async Task<ActionResult> Index()
        {


            if (_currentUser == null)
            {
                FormsAuthentication.SignOut();
                RedirectToAction("Login", "Account");
            }

            var user = new MainPageViewModel
            {
                PathPhoto = "~/Content/Home/nophoto.jpg",
                Name = _currentUser.Name,
                Surname = _currentUser.Surname,
                DateOfBirth = _currentUser.DateOfBirth
            };
            
            ViewBag.Title = _currentUser.Name + " " + _currentUser.Surname;

            return View(user);
        }

        public async Task<ActionResult> EditUserData()
        {
            EditProfileViewModel updeteUser = new EditProfileViewModel
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
        public async Task<ActionResult> EditUserData(EditProfileViewModel newData)
        {
            ViewBag.Message = "Ошибка в заполнении данных";

            if (ModelState.IsValid)
            {
                if (await UserData.db.WorkWithUser.CheckExistenceEmail(newData.Email, newData.Login))
                {
                    ViewBag.Message = "Указанный адрес электронной почты уже зарегистрирован в системе";

                    return View(newData);
                }

                if (!await UserData.db.WorkWithUser.UpdateUser(newData))
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
    }
}