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
        bool ChangePassword(ChangePasswordViewModel model);
        UserEntity GetUserByLoginOrEmail(string login);
        UserEntity GetUserByLoginOrEmailIncluding(string login);
        byte[] GetUserMainPhoto(string login);
        bool SaveNewCurrentUserMainPhoto(HttpPostedFileBase photo, UserEntity user);
        IEnumerable<UsersViewModel> GetAllUsers(int user);
        string AddRequestToFriendList(int user, int id);
        UsersViewModel GetUserPage(UserEntity mainUser, int id);
        IEnumerable<UsersViewModel> GetMyFriends(int user);
        IEnumerable<UsersViewModel> GetRequests(int user);
        IEnumerable<UsersViewModel> GetMyRequests(int user);
        IEnumerable<DialogsViewModel> GetAllDialogs(int user);
    }
}
