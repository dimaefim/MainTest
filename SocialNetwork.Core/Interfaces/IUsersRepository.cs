using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        bool CheckExistenceUser(string login, string password);
        bool CheckExistenceEmailOrLogin(string login, string email);
        bool CheckExistenceEmail(string email, string login);
        bool AddNewUser(RegistrationViewModel user);
        bool UpdateUser(EditProfileViewModel user);
        UserEntity GetUserByLoginOrEmail(string login);
    }
}
