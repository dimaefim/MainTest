using System.Web;
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
    public class FriendsController : BaseController
    {
        private readonly IFriendsRepository _friendsRepository = NinjectBindings.Instance.Get<IFriendsRepository>();

        public ActionResult ShowAllUsers()
        {
            return View();
        }

        public JsonResult GetAllUsers()
        {
            return Json(_friendsRepository.GetAllUsers(CurrentUser.Id));
        }

        public JsonResult GetMyFriends()
        {
            return Json(_friendsRepository.GetMyFriends(CurrentUser.Id));
        }

        public JsonResult GetRequests()
        {
            return Json(_friendsRepository.GetRequests(CurrentUser.Id));
        }

        public JsonResult GetMyRequests()
        {
            return Json(_friendsRepository.GetMyRequests(CurrentUser.Id));
        }

        public JsonResult AddRequestToFriendList(int id)
        {
            var obj = new { response = _friendsRepository.AddRequestToFriendList(CurrentUser.Id, id) };

            return Json(obj);
        }

        public ActionResult ShowMyFriends()
        {
            return View();
        }

        public ActionResult AddRequestToFriendListFromUserPage(int id = 0)
        {
            if (_friendsRepository.AddRequestToFriendList(CurrentUser.Id, id).Equals("false"))
            {
                return RedirectToAction("ErrorCode500", "Error");
            }

            return RedirectToAction("ShowUserPage", "Home", new { id = id });
        }
    }
}