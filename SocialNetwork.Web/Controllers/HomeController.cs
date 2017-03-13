using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using SocialNetwork.Core.Home;
using SocialNetwork.Core.Repository;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            _currentUser = await UserData.GetUserByLoginOrEmail(User.Identity.Name);

            if (_currentUser == null)
            {
                FormsAuthentication.SignOut();
                RedirectToAction("Login", "Account");
            }

            MainPageViewModel user = new MainPageViewModel
            {
                PathPhoto = "~/Content/Home/nophoto.jpg",
                Name = _currentUser.Name,
                Surname = _currentUser.Surname,
                DateOfBirth = _currentUser.DateOfBirth
            };
            

            ViewBag.Title = _currentUser.Name + " " + _currentUser.Surname;

            return View(user);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            if (Request.Cookies["SN_AUTH_COOKIES"] != null)
            {
                Response.Cookies["SN_AUTH_COOKIES"].Expires = DateTime.Now.AddDays(-1);
            }

            return RedirectToAction("Login", "Account");
        }

        private UserEntity _currentUser;
    }
}