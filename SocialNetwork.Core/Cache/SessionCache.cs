using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Description;
using System.Web.UI;
using SocialNetwork.Core.UnitOfWork;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.Core.Cache
{
    public static class SessionCache
    {
        public static UserEntity GetCurrentUser(string login)
        {
            /*cache.SetExpires(DateTime.Now.AddMinutes(30));
            cache.SetCacheability(HttpCacheability.Public);
            cache.
            cache.VaryByParams(login);*/

            Some(login);

            return _currentUser;
        }

        public static async void Some(string login)
        {
            _currentUser = await UserData.db.WorkWithUser.GetUserByLoginOrEmail(login);
        }

        private static UserEntity _currentUser;
        private static HttpCachePolicy cache;
    }
}
