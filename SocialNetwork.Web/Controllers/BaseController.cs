using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using SocialNetwork.Core.Cache;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.Web.Controllers
{
    public class BaseController : Controller
    {
        protected UserEntity _currentUser;

        public BaseController()
        {
            ViewBag.RenderMenu = true;
            SetCurrentUser();
            ViewBag.UserId = _currentUser.Id;
        }

        private void SetCurrentUser()
        {
            _currentUser = SessionCache.CurrentUser;
        }
    }
}