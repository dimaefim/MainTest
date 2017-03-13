using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Core.Repository;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.Core.Home
{
    public static class UserData
    {
        static UserData()
        {
            db = new SocialNetworkUW();
        }

        public static async Task<UserEntity> GetUserByLoginOrEmail(string login)
        {
            await UpdateData();

            return allUsers.FirstOrDefault(item => item.Login == login || item.Email == login);
        }

        private static async Task UpdateData()
        {
            allUsers = await db.Users.GetAllItems();
        }

        private static SocialNetworkUW db;
        private static IEnumerable<UserEntity> allUsers;
    }
}
