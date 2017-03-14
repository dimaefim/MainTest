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

        public static async Task<bool> CheckExistenceUser(RegistrationViewModel user)
        {
            var allUsers = await db.Users.GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
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

        public static async Task<bool> AddNewUser(RegistrationViewModel user)
        {
            try
            {
                var newUser = new UserEntity
                {
                    Login = user.Login,
                    Password = user.Password,
                    Name = user.Name,
                    Surname = user.Surname,
                    Patronymic = user.Patronymic.Length == 0 ? "Undefined" : user.Patronymic,
                    Email = user.Email,
                    DateOfBirth = user.DateOfBirth,
                    IsDeleted = false,
                    UserLastLoginDate = DateTime.Now
                };

                newUser.UserRoles.Add(new UsersInRolesEntity {RoleId = (int)RolesEnum.User, User = newUser});

                newUser.Settings = new UserSettingsEntity
                {
                    aboutMe = "",
                    User = newUser
                };

                await db.Users.CreateNewItem(newUser);
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
