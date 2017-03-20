using System.Threading.Tasks;
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
        private readonly UserEntity _currentUser;
        private readonly IUsersRepository _usersRepository = NinjectBindings.Instance.Get<IUsersRepository>();

        public HomeController()
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
                PathPhoto = "~/Content/Home/nophoto.jpg",
                Name = _currentUser.Name,
                Surname = _currentUser.Surname,
                DateOfBirth = _currentUser.DateOfBirth
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
        public async Task<ActionResult> EditUserData(EditProfileViewModel newData)
        {
            ViewBag.Message = "Ошибка в заполнении данных";

            if (ModelState.IsValid)
            {
                if (await _usersRepository.CheckExistenceEmailAsync(newData.Email, newData.Login))
                {
                    ViewBag.Message = "Указанный адрес электронной почты уже зарегистрирован в системе";

                    return View(newData);
                }

                if (!await _usersRepository.UpdateUserAsync(newData))
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