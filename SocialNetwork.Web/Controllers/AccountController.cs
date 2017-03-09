using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using SocialNetwork.Core.Account;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (!await UserLogin.CheckExistenceUser(user))
                {
                    ViewBag.Message = "Неверный логин или пароль";

                    return View(user);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
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

                return RedirectToAction("Index", "Home");
            }

            return View(newUser);
        }
    }
}
