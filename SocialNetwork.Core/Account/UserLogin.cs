using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Core.Repository;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Account
{
    public static class UserLogin
    {
        static UserLogin()
        {
            db = new SocialNetworkUW();
        }
        public static async Task<bool> CheckExistenceUser(LoginViewModel user)
        {
            var allUsers = await db.Users.GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                    (item.Login == user.Login && item.Password == user.Password) || 
                    (item.Email == user.Login && item.Password == user.Password)
                );

            return searchedUser != null;
        }

        private static readonly SocialNetworkUW db;
    }
}
