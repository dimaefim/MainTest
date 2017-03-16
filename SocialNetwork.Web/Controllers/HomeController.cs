using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Windows.Forms;
using SocialNetwork.Core.Account;
using SocialNetwork.Core.Home;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserEntity _currentUser;

        public HomeController()
        {
            
        }

        private async Task SetCurrentUser()
        {
            _currentUser = await UserData.GetUserByLoginOrEmail(User.Identity.Name);
        }

        public async Task<ActionResult> Index()
        {
            await SetCurrentUser();

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
            await SetCurrentUser();

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
            await SetCurrentUser();

            ViewBag.Message = "Ошибка в заполнении данных";

            if (ModelState.IsValid)
            {
                if (await UserData.CheckExistenceEmail(newData))
                {
                    ViewBag.Message = "Указанный адрес электронной почты уже зарегистрирован в системе";

                    return View(newData);
                }

                _currentUser.Name = newData.Name;
                _currentUser.Surname = newData.Surname;
                _currentUser.Patronymic = newData.Patronymic;
                _currentUser.Email = newData.Email;
                _currentUser.DateOfBirth = newData.DateOfBirth;
                _currentUser.Settings.aboutMe = newData.AboutMe;

                if (!await UserData.UpdateUser(_currentUser))
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