using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Ninject;
using SocialNetwork.Core.Cache;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;
using SocialNetwork.Models.Enums;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Repository
{
    public class UsersRepository : UserRepository, IUsersRepository
    {
        private readonly IFilesRepository _filesRepository = NinjectBindings.Instance.Get<IFilesRepository>();

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

        public async Task<byte[]> GetUserMainPhotoAsync(string login)
        {
            var searchedUser = await GetUserByLoginOrEmailAsync(login);

            byte[] photo = File.ReadAllBytes("F://Git Repository//SocialNetwork//SocialNetwork.Web//Content//Home/nophoto.jpg");

            if (searchedUser != null && 
                searchedUser.Settings.Files != null && 
                searchedUser.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto")) != null)
            {
                photo = searchedUser.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto")).Content;
            }

            return photo;
        }

        public async Task<bool> SaveNewUserMainPhotoAsync(HttpPostedFileBase photo, string login)
        {
            try
            {
                var searchedUser = await GetUserByLoginOrEmailAsync(login);

                if (searchedUser == null)
                {
                    return false;
                }

                if (searchedUser.Settings.Files != null)
                {
                    var files = searchedUser.Settings.Files;

                    if (!await _filesRepository.SaveNewUserAvatarAsync(searchedUser, photo))
                    {
                        return false;
                    }

                    SessionCache.CurrentUser = searchedUser;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<MainPageViewModel>> GetAllUsersAsync()
        {
            var allUsers = await GetAllItemsAsync();

            byte[] photo = File.ReadAllBytes("F://Git Repository//SocialNetwork//SocialNetwork.Web//Content//Home/nophoto.jpg");

            IEnumerable<MainPageViewModel> result = allUsers.Select(item => new MainPageViewModel
            {
                Name = item.Name,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth,
                AboutMe = item.Settings.aboutMe,
                MainPhoto = item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")) == null ? photo :
                    item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")).Content
            });

            return result;
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

        public byte[] GetUserMainPhoto(string login)
        {
            var searchedUser = GetUserByLoginOrEmail(login);

            byte[] photo = File.ReadAllBytes("F://Git Repository//SocialNetwork//SocialNetwork.Web//Content//Home/nophoto.jpg");

            if (searchedUser != null)
            {
                if (searchedUser.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto")) != null)
                {
                    photo = searchedUser.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto")).Content;
                }
            }

            return photo;
        }

        public bool SaveNewUserMainPhoto(HttpPostedFileBase photo, string login)
        {
            try
            {
                var searchedUser = GetUserByLoginOrEmail(login);

                if (searchedUser == null)
                {
                    return false;
                }

                if (searchedUser.Settings.Files != null)
                {
                    var files = searchedUser.Settings.Files;

                    if (!_filesRepository.SaveNewUserAvatar(searchedUser, photo))
                    {
                        return false;
                    }

                    SessionCache.CurrentUser = searchedUser;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<MainPageViewModel> GetAllUsers()
        {
            var allUsers = GetAllItems();

            byte[] photo = File.ReadAllBytes("F://Git Repository//SocialNetwork//SocialNetwork.Web//Content//Home/nophoto.jpg");

            IEnumerable<MainPageViewModel> result = allUsers.Select(item => new MainPageViewModel
            {
                Name = item.Name,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth,
                AboutMe = item.Settings.aboutMe,
                MainPhoto = item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")) == null ? photo :
                    item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")).Content
            });

            return result;
        }
    }
}
