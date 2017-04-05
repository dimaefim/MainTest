using System.Web.Mvc;
using System.Web.Security;
using Ninject;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUsersRepository _usersRepository = NinjectBindings.Instance.Get<IUsersRepository>();

        public AccountController()
        {
            ViewBag.RenderMenu = false;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel user)
        {
            if (Request.IsAuthenticated)
                FormsAuthentication.SignOut();

            if (ModelState.IsValid)
            {
                if (!_usersRepository.CheckExistenceUser(user.Login, user.Password))
                {
                    ViewBag.Message = "Неверный логин или пароль";

                    return View(user);
                }

                FormsAuthentication.SetAuthCookie(user.Login, user.RememberMe);

                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registration(RegistrationViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                if (_usersRepository.CheckExistenceEmailOrLogin(newUser.Login, newUser.Email))
                {
                    ViewBag.Message = "Пользователь с таким логином или адресом электронной почты уже существует";

                    return View(newUser);
                }

                if (!_usersRepository.AddNewUser(newUser))
                {
                    ViewBag.Message = "Ошибка создания пользователя";

                    return View(newUser);
                }

                return RedirectToAction("Login", "Account");
            }

            return View(newUser);
        }
    }
}