using System;
using System.Globalization;
using System.Web;
using System.Web.Caching;
using Ninject;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.Core.Cache
{
    public static class SessionCache
    {
        private const string KeyCurrentUser = "CurrentUser";

        public static UserEntity CurrentUser
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var user = HttpContext.Current.Cache.Get(KeyCurrentUser + HttpContext.Current.User.Identity.Name
                        .ToLower(CultureInfo.InvariantCulture)) as UserEntity;

                    if (user == null)
                    {
                        var userRepository = NinjectBindings.Instance.Get<IUsersRepository>();
                        user = userRepository.GetUserByLoginOrEmail(HttpContext.Current.User.Identity.Name);
                        AddUserToCache(user);
                    }

                    return user;
                }

                return null;
            }

            set { AddUserToCache(value); }
        }

        private static void AddUserToCache(UserEntity user)
        {
            if (user != null)
            {
                HttpContext.Current.Cache.Remove(KeyCurrentUser + user.Login.ToLower(CultureInfo.InvariantCulture));
                HttpContext.Current.Cache.Add(KeyCurrentUser + user.Login.ToLower(CultureInfo.InvariantCulture),
                    user,
                    null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(30),
                    CacheItemPriority.AboveNormal,
                    null);
            }
        }

        public static void UpdateCurrentUser()
        {
            var userRepository = NinjectBindings.Instance.Get<IUsersRepository>();
            var user = userRepository.GetUserByLoginOrEmail(HttpContext.Current.User.Identity.Name);
            AddUserToCache(user);
        }
    }
}
