using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Core.Repository;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Models;

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

        public static async Task<bool> CheckExistenceEmail(EditProfileViewModel user)
        {
            await UpdateData();

            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                        item.Email == user.Email && item.Login != user.Login
                );

            if (searchedUser == null)
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> UpdateUser(UserEntity user)
        {
            try
            {
                await db.Users.UpdateItem(user);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static SocialNetworkUW db;
        private static IEnumerable<UserEntity> allUsers;
    }
}
