﻿using System;
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

                var friend = GetItemById(id);

                switch (GetUserStatus(updatedUser, friend))
                {
                    case FriendStatusEnum.Me:

                    return "me";

                    case FriendStatusEnum.Friends:
                        _context.Friends.Remove(
                            _context.Friends.Any(t => t.UserId == updatedUser.Id && t.FriendId == friend.Id) 
                                ? _context.Friends.FirstOrDefault(t => t.UserId == updatedUser.Id && t.FriendId == friend.Id) 
                                : _context.Friends.FirstOrDefault(t => t.UserId == friend.Id && t.FriendId == updatedUser.Id)
                            );

                        _context.SaveChanges();

                        return "no friend";

                    case FriendStatusEnum.WaitAccept:
                        _context.Friends.Remove(
                            _context.Friends.FirstOrDefault(t => t.UserId == updatedUser.Id && t.FriendId == friend.Id)
                            );

                        _context.SaveChanges();

                        return "no friend";

                    case FriendStatusEnum.UserWaitAccept:
                        _context.Friends.FirstOrDefault(item => item.UserId == friend.Id && item.FriendId == updatedUser.Id)
                            .IsFriends = true;
                        _context.Friends.FirstOrDefault(item => item.UserId == friend.Id && item.FriendId == updatedUser.Id)
                            .DataConfirm = DateTime.Now;

                        _context.SaveChanges();

                        return "i accept";

                    case FriendStatusEnum.NoFriends:
                        _context.Friends.Add(new FriendsEntity
                        {
                            RequestDate = DateTime.Now,
                            IsFriends = false,
                            FriendId = friend.Id,
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
            return mainUser.Id == secondUser.Id
                ? FriendStatusEnum.Me
                : _context.Friends.Any(
                    t =>
                        (t.FriendId == secondUser.Id || t.FriendId == mainUser.Id) &&
                        (t.UserId == secondUser.Id || t.UserId == mainUser.Id) && t.IsFriends)
                    ? FriendStatusEnum.Friends
                    : _context.Friends.Any(t => t.UserId == mainUser.Id && t.FriendId == secondUser.Id)
                        ? FriendStatusEnum.WaitAccept
                        : _context.Friends.Any(t => t.UserId == secondUser.Id && t.FriendId == mainUser.Id)
                            ? FriendStatusEnum.UserWaitAccept
                            : FriendStatusEnum.NoFriends;
        }

        private IEnumerable<UsersViewModel> GetAllUsersStatus(UserEntity user)
        {
            var photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

            IEnumerable<UsersViewModel> result = _context.Users.Where(y => y.Id != user.Id).Select(item => new UsersViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth,
                AboutMe = item.Settings.aboutMe,
                MainPhoto = item.Settings.Files.Any(i => i.Notes.Equals("MainPhoto"))
                    ? item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")).Content :
                    photo,
                Status = user.Id == item.Id
                ? FriendStatusEnum.Me
                : _context.Friends.Any(
                    t =>
                        (t.FriendId == item.Id || t.FriendId == user.Id) &&
                        (t.UserId == item.Id || t.UserId == user.Id) && t.IsFriends)
                    ? FriendStatusEnum.Friends
                    : _context.Friends.Any(t => t.UserId == user.Id && t.FriendId == item.Id)
                        ? FriendStatusEnum.WaitAccept
                        : _context.Friends.Any(t => t.UserId == item.Id && t.FriendId == user.Id)
                            ? FriendStatusEnum.UserWaitAccept
                            : FriendStatusEnum.NoFriends
        });

            return result;
        }
    }
}
