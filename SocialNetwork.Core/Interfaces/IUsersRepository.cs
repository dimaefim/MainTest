using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Interfaces
{
    public interface IUsersRepository : IRepository<UserEntity>
    {
        bool CheckExistenceUser(string login, string password);
        bool CheckExistenceEmailOrLogin(string login, string email);
        bool CheckExistenceEmail(string email, string login);
        bool AddNewUser(RegistrationViewModel user);
        bool UpdateCurrentUser(EditProfileViewModel user);
        UserEntity GetUserByLoginOrEmail(string login);
        byte[] GetUserMainPhoto(string login);
        bool SaveNewCurrentUserMainPhoto(HttpPostedFileBase photo, UserEntity user);
        IEnumerable<UsersViewModel> GetAllUsers(UserEntity user);
        string AddRequestToFriendList(UserEntity user, int id);
        UsersViewModel GetUserPage(UserEntity mainUser, int id);
        IEnumerable<UsersViewModel> GetMyFriends(UserEntity user);
        IEnumerable<UsersViewModel> GetRequests(UserEntity user);
        IEnumerable<UsersViewModel> GetMyRequests(UserEntity user);
    }
}
