using System.Web.Mvc;
using SocialNetwork.Core.Cache;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected UserEntity CurrentUser;

        protected BaseController()
        {
            SessionCache.UpdateCurrentUser();
            ViewBag.RenderMenu = true;
            SetCurrentUser();
            if (CurrentUser != null)
                ViewBag.UserId = CurrentUser.Id;
        }

        private void SetCurrentUser()
        {
            CurrentUser = SessionCache.CurrentUser;
        }
    }
}