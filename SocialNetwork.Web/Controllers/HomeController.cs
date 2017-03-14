using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using SocialNetwork.Core.Home;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserEntity _currentUser;

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
    }
}