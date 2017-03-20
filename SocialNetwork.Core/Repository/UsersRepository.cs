using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;
using SocialNetwork.Models.Enums;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Repository
{
    public class UsersRepository : UserRepository, IUsersRepository
    {
        public UsersRepository(SocialNetworkContext context) : base (context)
        {}

        public async Task<bool> CheckExistenceUser(string login, string password)
        {
            var allUsers = await GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                    (item.Login == login && item.Password == password) ||
                    (item.Email == login && item.Password == password)
                );

            return searchedUser != null;
        }

        public async Task<bool> CheckExistenceEmailOrLogin(string login, string email)
        {
            var allUsers = await GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                        item.Login == login || item.Email == email
                );

            return searchedUser != null;
        }

        public async Task<bool> CheckExistenceEmail(string email, string login)
        {
            var allUsers = await GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                        item.Email == email && item.Login != login
                );

            return searchedUser != null;
        }

        public async Task<bool> AddNewUser(RegistrationViewModel user)
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

                newUser.UserRoles.Add(new UsersInRolesEntity { RoleId = (int)RolesEnum.User, User = newUser });

                newUser.Settings = new UserSettingsEntity
                {
                    aboutMe = "",
                    User = newUser
                };

                await CreateNewItem(newUser);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateUser(EditProfileViewModel user)
        {
            try
            {
                var updatedUser = await GetUserByLoginOrEmail(user.Login);

                updatedUser.Name = user.Name;
                updatedUser.Surname = user.Surname;
                updatedUser.Patronymic = user.Patronymic;
                updatedUser.Email = user.Email;
                updatedUser.DateOfBirth = user.DateOfBirth;
                updatedUser.Settings.aboutMe = user.AboutMe;

                await UpdateItem(updatedUser);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<UserEntity> GetUserByLoginOrEmail(string login)
        {
            var allUsers = await GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item => 
                        item.Login == login || item.Email == login
                );

            return searchedUser;
        }
    }
}
