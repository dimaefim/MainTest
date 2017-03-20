using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocialNetwork.Core.Cache;
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

        public async Task<bool> CheckExistenceUserAsync(string login, string password)
        {
            var allUsers = await GetAllItemsAsync();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                    (item.Login == login && item.Password == password) ||
                    (item.Email == login && item.Password == password)
                );

            return searchedUser != null;
        }

        public async Task<bool> CheckExistenceEmailOrLoginAsync(string login, string email)
        {
            var allUsers = await GetAllItemsAsync();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                        item.Login == login || item.Email == email
                );

            return searchedUser != null;
        }

        public async Task<bool> CheckExistenceEmailAsync(string email, string login)
        {
            var allUsers = await GetAllItemsAsync();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                        item.Email == email && item.Login != login
                );

            return searchedUser != null;
        }

        public async Task<bool> AddNewUserAsync(RegistrationViewModel user)
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

                await CreateNewItemAsync(newUser);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateUserAsync(EditProfileViewModel user)
        {
            try
            {
                var updatedUser = await GetUserByLoginOrEmailAsync(user.Login);

                updatedUser.Name = user.Name;
                updatedUser.Surname = user.Surname;
                updatedUser.Patronymic = user.Patronymic;
                updatedUser.Email = user.Email;
                updatedUser.DateOfBirth = user.DateOfBirth;
                updatedUser.Settings.aboutMe = user.AboutMe;

                await UpdateItemAsync(updatedUser);

                SessionCache.CurrentUser = updatedUser;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<UserEntity> GetUserByLoginOrEmailAsync(string login)
        {
            var allUsers = await GetAllItemsAsync();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item => 
                        item.Login == login || item.Email == login
                );

            return searchedUser;
        }

        public bool CheckExistenceUser(string login, string password)
        {
            var allUsers = GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                    (item.Login == login && item.Password == password) ||
                    (item.Email == login && item.Password == password)
                );

            return searchedUser != null;
        }

        public bool CheckExistenceEmailOrLogin(string login, string email)
        {
            var allUsers = GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                        item.Login == login || item.Email == email
                );

            return searchedUser != null;
        }

        public bool CheckExistenceEmail(string email, string login)
        {
            var allUsers = GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                        item.Email == email && item.Login != login
                );

            return searchedUser != null;
        }

        public bool AddNewUser(RegistrationViewModel user)
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

                CreateNewItem(newUser);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool UpdateUser(EditProfileViewModel user)
        {
            try
            {
                var updatedUser = GetUserByLoginOrEmail(user.Login);

                updatedUser.Name = user.Name;
                updatedUser.Surname = user.Surname;
                updatedUser.Patronymic = user.Patronymic;
                updatedUser.Email = user.Email;
                updatedUser.DateOfBirth = user.DateOfBirth;
                updatedUser.Settings.aboutMe = user.AboutMe;

                UpdateItem(updatedUser);

                SessionCache.CurrentUser = updatedUser;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public UserEntity GetUserByLoginOrEmail(string login)
        {
            var allUsers = GetAllItems();
            var searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                        item.Login == login || item.Email == login
                );

            return searchedUser;
        }
    }
}
