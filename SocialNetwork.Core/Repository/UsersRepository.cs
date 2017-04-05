using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
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
                    (item.Login == login || item.Email == login) && item.Password == password
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
                    DateOfBirth = DateTime.Parse(user.DateOfBirth),
                    IsDeleted = false,
                    UserLastLoginDate = DateTime.Now
                };

                newUser.UserRoles.Add(new UsersInRolesEntity { RoleId = (int)RolesEnum.User, User = newUser });

                newUser.Settings = new UserSettingsEntity
                {
                    AboutMe = "",
                    User = newUser
                };

                CreateNewItem(newUser);

                byte[] photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

                _context.Files.Add(new FileEntity
                {
                    Name = "nophoto.jpg",
                    DateCreated = DateTime.Now,
                    MimeType = "image/*",
                    Notes = "MainPhoto",
                    Content = photo,
                    UserSettingsId = newUser.Settings.Id
                });

                _context.SaveChanges();
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
                updatedUser.DateOfBirth = DateTime.Parse(user.DateOfBirth);
                updatedUser.Settings.AboutMe = user.AboutMe;

                UpdateItem(updatedUser);

                SessionCache.UpdateCurrentUser();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var updatedUser = GetUserByLoginOrEmail(SessionCache.CurrentUser.Login);
                
                updatedUser.Password = model.NewPassword;

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

        public UserEntity GetUserByLoginOrEmailIncluding(string login)
        {
            return _context.Users.Include(item => item.FriendUsers)
                .Include(item => item.UserFriends)
                .Include(item => item.Settings)
                .Include(item => item.Settings.Files).SingleOrDefault(item => item.Login == login);
        }

        public byte[] GetUserMainPhoto(string login)
        {
            var searchedUser = GetUserByLoginOrEmail(login);

            if(!searchedUser.Settings.Files.Any(item => item.Notes.Equals("MainPhoto")))
            {
                return File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));
            }

            return searchedUser.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto")).Content;
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

                    SessionCache.UpdateCurrentUser();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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
                AboutMe = user.Settings.AboutMe,
                MainPhoto = GetUserMainPhoto(user.Login),
                Status = GetUserStatus(mainUser.Id, user.Id)
            };

            return model;
        }

        private FriendStatusEnum GetUserStatus(int mainUser, int secondUser)
        {
            return mainUser == secondUser
                ? FriendStatusEnum.Me
                : _context.Friends.Any(
                    t =>
                        (t.FriendId == secondUser || t.FriendId == mainUser) &&
                        (t.UserId == secondUser || t.UserId == mainUser) && t.IsFriends)
                    ? FriendStatusEnum.Friends
                    : _context.Friends.Any(t => t.UserId == mainUser && t.FriendId == secondUser)
                        ? FriendStatusEnum.WaitAccept
                        : _context.Friends.Any(t => t.UserId == secondUser && t.FriendId == mainUser)
                            ? FriendStatusEnum.UserWaitAccept
                            : FriendStatusEnum.NoFriends;
        }
    }
}