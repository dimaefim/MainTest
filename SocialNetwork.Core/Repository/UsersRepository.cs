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

        public bool CheckExistenceUser(string login, string password)
        {
            var searchedUser = _context.Users.FirstOrDefault
                (
                    item =>
                    (item.Login == login && item.Password == password) ||
                    (item.Email == login && item.Password == password)
                );

            return searchedUser != null;
        }

        public bool CheckExistenceEmailOrLogin(string login, string email)
        {
            var searchedUser = _context.Users.FirstOrDefault
                (
                    item =>
                        item.Login == login || item.Email == email
                );

            return searchedUser != null;
        }

        public bool CheckExistenceEmail(string email, string login)
        {
            var searchedUser = _context.Users.FirstOrDefault
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

        public bool UpdateCurrentUser(EditProfileViewModel user)
        {
            try
            {
                var updatedUser = GetUserByLoginOrEmail(user.Login);

                updatedUser.Name = user.Name;
                updatedUser.Surname = user.Surname;
                updatedUser.Patronymic = user.Patronymic;
                updatedUser.Email = user.Email;
                updatedUser.DateOfBirth = DateTime.Parse(user.DateOfBirthString);
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
            var searchedUser = _context.Users.FirstOrDefault
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

        public bool SaveNewCurrentUserMainPhoto(HttpPostedFileBase photo, UserEntity user)
        {
            try
            {
                var searchedUser = GetUserByLoginOrEmail(user.Login);

                if (searchedUser == null)
                {
                    return false;
                }

                if (searchedUser.Settings.Files != null)
                {
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
                DateOfBirth = user.DateOfBirth,
                AboutMe = user.Settings.aboutMe,
                MainPhoto = GetUserMainPhoto(user.Login),
                Status = GetUserStatus(mainUser, user)
            };

            return model;
        }

        private FriendStatusEnum GetUserStatus(UserEntity mainUser, UserEntity secondUser)
        {
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
            var photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

            var allUsers = GetAllItems();

            var result = allUsers.Select(item => new UsersViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth,
                AboutMe = item.Settings.aboutMe,
                MainPhoto = item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")) == null ? photo :
                    item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")).Content,
                Status = GetUserStatus(user, item)
            });

            return result;
        }
    }
}
