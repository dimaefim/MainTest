using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
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
                    DateOfBirth = DateTime.Parse(user.DateOfBirth),
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
                updatedUser.DateOfBirth = DateTime.Parse(user.DateOfBirth);
                updatedUser.Settings.aboutMe = user.AboutMe;

                await UpdateItemAsync(updatedUser);

                SessionCache.UpdateCurrentUser();
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

            byte[] photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

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

        public async Task<IEnumerable<UsersViewModel>> GetAllUsersAsync(UserEntity user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status != FriendStatusEnum.Me && item.Status != FriendStatusEnum.Friends);
        }

        public async Task<IEnumerable<UsersViewModel>> GetMyFriendsAsync(UserEntity user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.Friends);
        }

        public async Task<IEnumerable<UsersViewModel>> GetRequestsAsync(UserEntity user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.UserWaitAccept);
        }

        public async Task<IEnumerable<UsersViewModel>> GetMyRequestsAsync(UserEntity user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.WaitAccept);
        }

        public async Task<string> AddRequestToFriendListAsync(UserEntity user, int id)
        {
            try
            {
                var updatedUser = await GetUserByLoginOrEmailAsync(user.Login);

                var newFriend = await GetItemByIdAsync(id);

                switch (GetUserStatus(updatedUser, newFriend))
                {
                    case FriendStatusEnum.Me:

                        return "me";

                    case FriendStatusEnum.Friends:
                        _context.Friends.Remove(
                            updatedUser.UserFriends.FirstOrDefault(
                                item => item.UserId == updatedUser.Id && item.FriendId == newFriend.Id));

                        _context.Friends.FirstOrDefault(item => item.UserId == newFriend.Id && item.FriendId == updatedUser.Id)
                            .IsFriends = false;

                        await _context.SaveChangesAsync();

                        return "no friend";

                    case FriendStatusEnum.WaitAccept:
                        _context.Friends.Remove(
                            updatedUser.UserFriends.FirstOrDefault(
                                item => item.UserId == updatedUser.Id && item.FriendId == newFriend.Id));

                        await _context.SaveChangesAsync();

                        return "no friend";

                    case FriendStatusEnum.UserWaitAccept:
                        _context.Friends.Add(new FriendsEntity
                        {
                            RequestDate = DateTime.Now,
                            IsFriends = true,
                            FriendId = newFriend.Id,
                            UserId = updatedUser.Id
                        });

                        _context.Friends.FirstOrDefault(item => item.UserId == newFriend.Id && item.FriendId == updatedUser.Id)
                            .IsFriends = true;

                        await _context.SaveChangesAsync();

                        return "i accept";

                    case FriendStatusEnum.NoFriends:
                        _context.Friends.Add(new FriendsEntity
                        {
                            RequestDate = DateTime.Now,
                            IsFriends = false,
                            FriendId = newFriend.Id,
                            UserId = updatedUser.Id
                        });

                        await _context.SaveChangesAsync();

                        return "request";
                }
            }
            catch (Exception)
            {
                return "false";
            }

            return "false";
        }

        public async Task<UsersViewModel> GetUserPageAsync(UserEntity mainUser, int id)
        {
            var user = await GetItemByIdAsync(id);

            var model = new UsersViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                DateOfBirth = user.DateOfBirth.ToShortDateString(),
                AboutMe = user.Settings.aboutMe,
                MainPhoto = await GetUserMainPhotoAsync(user.Login),
                Status = GetUserStatus(mainUser, user)
            };

            return model;
        }

        private async Task<IEnumerable<UsersViewModel>> GetAllUsersStatusAsync(UserEntity user)
        {
            var allUsers = await GetAllItemsAsync();

            byte[] photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

            IEnumerable<UsersViewModel> result = allUsers.Select(item => new UsersViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth.ToShortDateString(),
                AboutMe = item.Settings.aboutMe,
                MainPhoto = item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")) == null ? photo :
                    item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")).Content,
                Status = GetUserStatus(user, item)
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
                    DateOfBirth = DateTime.Parse(user.DateOfBirth),
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
                updatedUser.DateOfBirth = DateTime.Parse(user.DateOfBirth);
                updatedUser.Settings.aboutMe = user.AboutMe;

                UpdateItem(updatedUser);

                SessionCache.UpdateCurrentUser();
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

            byte[] photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

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

        public IEnumerable<UsersViewModel> GetAllUsers(UserEntity user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status != FriendStatusEnum.Me && item.Status != FriendStatusEnum.Friends);
        }

        public IEnumerable<UsersViewModel> GetMyFriends(UserEntity user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.Friends);
        }

        public IEnumerable<UsersViewModel> GetRequests(UserEntity user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.UserWaitAccept);
        }

        public IEnumerable<UsersViewModel> GetMyRequests(UserEntity user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.WaitAccept);
        }

        public string AddRequestToFriendList(UserEntity user, int id)
        {
            try
            {
                var updatedUser = GetUserByLoginOrEmail(user.Login);

                var newFriend = GetItemById(id);

                switch (GetUserStatus(updatedUser, newFriend))
                {
                    case FriendStatusEnum.Me:

                    return "me";

                    case FriendStatusEnum.Friends:
                        _context.Friends.Remove(
                            updatedUser.UserFriends.FirstOrDefault(
                                item => item.UserId == updatedUser.Id && item.FriendId == newFriend.Id));

                        _context.Friends.FirstOrDefault(item => item.UserId == newFriend.Id && item.FriendId == updatedUser.Id)
                            .IsFriends = false;

                        _context.SaveChanges();

                        return "no friend";

                    case FriendStatusEnum.WaitAccept:
                        _context.Friends.Remove(
                            updatedUser.UserFriends.FirstOrDefault(
                                item => item.UserId == updatedUser.Id && item.FriendId == newFriend.Id));

                        _context.SaveChanges();

                        return "no friend";

                    case FriendStatusEnum.UserWaitAccept:
                        _context.Friends.Add(new FriendsEntity
                        {
                            RequestDate = DateTime.Now,
                            IsFriends = true,
                            FriendId = newFriend.Id,
                            UserId = updatedUser.Id
                        });

                        _context.Friends.FirstOrDefault(item => item.UserId == newFriend.Id && item.FriendId == updatedUser.Id)
                            .IsFriends = true;

                        _context.SaveChanges();

                        return "i accept";

                    case FriendStatusEnum.NoFriends:
                        _context.Friends.Add(new FriendsEntity
                        {
                            RequestDate = DateTime.Now,
                            IsFriends = false,
                            FriendId = newFriend.Id,
                            UserId = updatedUser.Id
                        });

                        _context.SaveChanges();

                        return "request";
                }
            }
            catch (Exception)
            {
                return "false";
            }

            return "false";
        }

        public UsersViewModel GetUserPage(UserEntity mainUser, int id)
        {
            var user = GetItemById(id);

            var model = new UsersViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                DateOfBirth = user.DateOfBirth.ToShortDateString(),
                AboutMe = user.Settings.aboutMe,
                MainPhoto = GetUserMainPhoto(user.Login),
                Status = GetUserStatus(mainUser, user)
            };

            return model;
        }

        private FriendStatusEnum GetUserStatus(UserEntity mainUser, UserEntity secondUser)
        {
            var allUsers = GetAllItems();

            mainUser = allUsers.FirstOrDefault(item => item.Id == mainUser.Id);
            secondUser = allUsers.FirstOrDefault(item => item.Id == secondUser.Id);

            if (mainUser.Id == secondUser.Id)
            {
                return FriendStatusEnum.Me;
            }

            if (mainUser.UserFriends.FirstOrDefault(item => item.FriendId == secondUser.Id) != null)
            {
                if (mainUser.UserFriends.FirstOrDefault(item => item.FriendId == secondUser.Id).IsFriends)
                {
                    return FriendStatusEnum.Friends;
                }

                return FriendStatusEnum.WaitAccept;
            }

            if (secondUser.UserFriends.FirstOrDefault(item => item.FriendId == mainUser.Id) != null)
            {
                return FriendStatusEnum.UserWaitAccept;
            }

            return FriendStatusEnum.NoFriends;
        }

        private IEnumerable<UsersViewModel> GetAllUsersStatus(UserEntity user)
        {
            var allUsers = GetAllItems();

            byte[] photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

            IEnumerable<UsersViewModel> result = allUsers.Select(item => new UsersViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth.ToShortDateString(),
                AboutMe = item.Settings.aboutMe,
                MainPhoto = item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")) == null ? photo :
                    item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")).Content,
                Status = GetUserStatus(user, item)
            });

            return result;
        }
    }
}
