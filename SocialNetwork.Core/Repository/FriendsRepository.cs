using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;
using SocialNetwork.Models.Enums;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Repository
{
    public class FriendsRepository : FriendRepository, IFriendsRepository
    {
        private readonly IUsersRepository _usersRepository = NinjectBindings.Instance.Get<IUsersRepository>();
        public FriendsRepository(SocialNetworkContext context) : base (context)
        { }

        public IEnumerable<UsersViewModel> GetAllUsers(int user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status != FriendStatusEnum.Me && item.Status != FriendStatusEnum.Friends);
        }

        public IEnumerable<UsersViewModel> GetMyFriends(int user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.Friends);
        }

        public IEnumerable<UsersViewModel> GetRequests(int user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.UserWaitAccept);
        }

        public IEnumerable<UsersViewModel> GetMyRequests(int user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.WaitAccept);
        }

        public string AddRequestToFriendList(int user, int id)
        {
            try
            {
                switch (GetUserStatus(user, id))
                {
                    case FriendStatusEnum.Me:

                        return "me";

                    case FriendStatusEnum.Friends:
                        var deletedFriend =
                            _context.Friends.FirstOrDefault(t => (t.FriendId == id || t.FriendId == user) &&
                                                                 (t.UserId == id || t.UserId == user) && t.IsFriends);

                        _context.Friends.Remove(deletedFriend);

                        _context.SaveChanges();

                        return "no friend";

                    case FriendStatusEnum.WaitAccept:
                        _context.Friends.Remove(
                            _context.Friends.FirstOrDefault(t => t.UserId == user && t.FriendId == id)
                            );

                        _context.SaveChanges();

                        return "no friend";

                    case FriendStatusEnum.UserWaitAccept:
                        var friend = _context.Friends.FirstOrDefault(
                            item => item.UserId == id && item.FriendId == user);

                        friend.IsFriends = true;
                        friend.DataConfirm = DateTime.Now;

                        _context.SaveChanges();

                        return "i accept";

                    case FriendStatusEnum.NoFriends:
                        _context.Friends.Add(new FriendsEntity
                        {
                            RequestDate = DateTime.Now,
                            IsFriends = false,
                            FriendId = id,
                            UserId = user
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

        public FriendStatusEnum GetUserStatus(int mainUser, int secondUser)
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

        public IEnumerable<UsersViewModel> GetAllUsersStatus(int user)
        {
            var users = _context.Users.Where(y => y.Id != user).ToList();

            IEnumerable<UsersViewModel> result = users.Select(item => new UsersViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth,
                AboutMe = item.Settings.AboutMe,
                MainPhoto = _usersRepository.GetUserMainPhoto(item.Login),
                Status = user == item.Id
                ? FriendStatusEnum.Me
                : _context.Friends.Any(
                    t =>
                        (t.FriendId == item.Id || t.FriendId == user) &&
                        (t.UserId == item.Id || t.UserId == user) && t.IsFriends)
                    ? FriendStatusEnum.Friends
                    : _context.Friends.Any(t => t.UserId == user && t.FriendId == item.Id)
                        ? FriendStatusEnum.WaitAccept
                        : _context.Friends.Any(t => t.UserId == item.Id && t.FriendId == user)
                            ? FriendStatusEnum.UserWaitAccept
                            : FriendStatusEnum.NoFriends
            });

            return result;
        }
    }
}
