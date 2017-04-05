using System.Collections.Generic;
using System.Web;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Models;
using SocialNetwork.Models.Enums;

namespace SocialNetwork.Core.Interfaces
{
    public interface IFriendsRepository
    {
        IEnumerable<UsersViewModel> GetAllUsers(int user);
        string AddRequestToFriendList(int user, int id);
        IEnumerable<UsersViewModel> GetMyFriends(int user);
        IEnumerable<UsersViewModel> GetRequests(int user);
        IEnumerable<UsersViewModel> GetMyRequests(int user);
        FriendStatusEnum GetUserStatus(int mainUser, int secondUser);
        IEnumerable<UsersViewModel> GetAllUsersStatus(int user);
    }
}
