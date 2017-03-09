using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Core.Repository;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Enums;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Account
{
    public static class UserRegistration
    {
        static UserRegistration()
        {
            db = new SocialNetworkUW();
        }
        public static async Task<bool> Check_Existence_User(RegistrationViewModel user)
        {
            IEnumerable<UserEntity> allUsers = await db.Users.GetAllItems();
            UserEntity searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                    item.Login == user.Login || item.Email == user.Email
                );

            if (searchedUser == null)
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> Add_New_User(RegistrationViewModel user)
        {
            UserEntity newUser = new UserEntity
            {
                Login = user.Login,
                Password = user.Password,
                Name = user.Name,
                Surname = user.Surname,
                Patronymic = user.Patronymic.Length == 0 ? "" : user.Patronymic,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                IsDeleted = false,
                UserLastLoginDate = DateTime.Now
            };

            try
            {
                await db.Users.CreateNewItem(newUser);

                IEnumerable<UserEntity> allUsers = await db.Users.GetAllItems();
                UserEntity addedUser = allUsers.Single(item => item.Login == newUser.Email);

                
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static SocialNetworkUW db;
    }
}
