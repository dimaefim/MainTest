using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Ninject;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.Core.Repository;
using SocialNetwork.Core.UnitOfWork;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUsersRepository _usersRepository = NinjectBindings.Instance.Get<IUsersRepository>();

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel user)
        {
            if (Request.IsAuthenticated)
                FormsAuthentication.SignOut();

            if (ModelState.IsValid)
            {
                if (!await _usersRepository.CheckExistenceUserAsync(user.Login, user.Password))
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
        public async Task<ActionResult> Registration(RegistrationViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                if (await _usersRepository.CheckExistenceEmailOrLoginAsync(newUser.Login, newUser.Email))
                {
                    ViewBag.Message = "Пользователь с таким логином или адресом электронной почты уже существует";

                    return View(newUser);
                }

                if (!await _usersRepository.AddNewUserAsync(newUser))
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