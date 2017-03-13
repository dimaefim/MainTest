using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using SocialNetwork.Core.Account;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
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
                if (!await UserLogin.CheckExistenceUser(user))
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
                if (await UserRegistration.CheckExistenceUser(newUser))
                {
                    ViewBag.Message = "Пользователь с таким логином или адресом электронной почты уже существует";

                    return View(newUser);
                }

                if (!await UserRegistration.AddNewUser(newUser))
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