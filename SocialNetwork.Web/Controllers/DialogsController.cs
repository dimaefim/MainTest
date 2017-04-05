using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ninject;
using SocialNetwork.Core.Cache;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using System.Windows.Forms;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class DialogsController : Controller
    {
        private readonly IDialogsRepository _dialogsRepository = NinjectBindings.Instance.Get<IDialogsRepository>();
        private UserEntity _currentUser;

        public DialogsController()
        {
            ViewBag.RenderMenu = true;
            SetCurrentUser();
        }

        private void SetCurrentUser()
        {
            _currentUser = SessionCache.CurrentUser;
        }

        public ActionResult MyDialogs()
        {
            return View();
        }

        public JsonResult GetAllDialogs()
        {
            return Json(_dialogsRepository.GetAllDialogs(_currentUser.Id));
        }

        public ActionResult OpenDialog(int id = 0)
        {
            ViewBag.Users = new int[] { _currentUser.Id, id };

            return View();
        }

        public ActionResult OpenDialogByDialogId(int id = 0)
        {
            ViewBag.Users = _dialogsRepository.GetDialogUsers(id);

            return View();
        }

        public JsonResult GetMessages(int[] users)
        {
            return Json(new { messages = _dialogsRepository.GetMessages(users), userData = _dialogsRepository.GetUserMessageViewModel(users) });
        }

        [HttpPost]
        public JsonResult SendMessage(int[] users, string message)
        {
            return Json(_dialogsRepository.SendMessage(users, message, _currentUser.Id)
                ? new { response = true }
                : new { response = false });
        }
    }
}