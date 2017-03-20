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
        Task<bool> CheckExistenceUser(string login, string password);
        Task<bool> CheckExistenceEmailOrLogin(string login, string email);
        Task<bool> CheckExistenceEmail(string email, string login);
        Task<bool> AddNewUser(RegistrationViewModel user);
        Task<bool> UpdateUser(EditProfileViewModel user);
        Task<UserEntity> GetUserByLoginOrEmail(string login);
    }
}
