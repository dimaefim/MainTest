using System.Web.Mvc;
using Ninject;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    public class DialogsController : BaseController
    {
        private readonly IDialogsRepository _dialogsRepository = NinjectBindings.Instance.Get<IDialogsRepository>();

        public ActionResult MyDialogs()
        {
            return View();
        }

        public JsonResult GetAllDialogs()
        {
            return Json(_dialogsRepository.GetAllDialogs(CurrentUser.Id));
        }

        public ActionResult OpenDialog(int id = 0)
        {
            ViewBag.Users = new int[] { CurrentUser.Id, id };

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
            return Json(_dialogsRepository.SendMessage(users, message, CurrentUser.Id)
                ? new { response = true }
                : new { response = false });
        }
    }
}