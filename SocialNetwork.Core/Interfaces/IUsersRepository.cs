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
        Task<bool> CheckExistenceUserAsync(string login, string password);
        Task<bool> CheckExistenceEmailOrLoginAsync(string login, string email);
        Task<bool> CheckExistenceEmailAsync(string email, string login);
        Task<bool> AddNewUserAsync(RegistrationViewModel user);
        Task<bool> UpdateUserAsync(EditProfileViewModel user);
        Task<UserEntity> GetUserByLoginOrEmailAsync(string login);
        Task<byte[]> GetUserMainPhotoAsync(string login);
        Task<bool> SaveNewUserMainPhotoAsync(HttpPostedFileBase photo, string login);
        Task<IEnumerable<UsersViewModel>> GetAllUsersAsync(UserEntity user);
        Task<string> AddRequestToFriendListAsync(UserEntity user, int id);
        Task<UsersViewModel> GetUserPageAsync(UserEntity mainUser, int id);
        Task<IEnumerable<UsersViewModel>> GetMyFriendsAsync(UserEntity user);
        Task<IEnumerable<UsersViewModel>> GetRequestsAsync(UserEntity user);
        Task<IEnumerable<UsersViewModel>> GetMyRequestsAsync(UserEntity user);

        bool CheckExistenceUser(string login, string password);
        bool CheckExistenceEmailOrLogin(string login, string email);
        bool CheckExistenceEmail(string email, string login);
        bool AddNewUser(RegistrationViewModel user);
        bool UpdateUser(EditProfileViewModel user);
        UserEntity GetUserByLoginOrEmail(string login);
        byte[] GetUserMainPhoto(string login);
        bool SaveNewUserMainPhoto(HttpPostedFileBase photo, string login);
        IEnumerable<UsersViewModel> GetAllUsers(UserEntity user);
        string AddRequestToFriendList(UserEntity user, int id);
        UsersViewModel GetUserPage(UserEntity mainUser, int id);
        IEnumerable<UsersViewModel> GetMyFriends(UserEntity user);
        IEnumerable<UsersViewModel> GetRequests(UserEntity user);
        IEnumerable<UsersViewModel> GetMyRequests(UserEntity user);
    }
}
