using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.Core.Interfaces
{
    public interface IFilesRepository : IRepository<FileEntity>
    {
        Task<bool> SaveNewUserAvatarAsync(UserEntity user, HttpPostedFileBase uploadImage);
        bool SaveNewUserAvatar(UserEntity user, HttpPostedFileBase uploadImage);
    }
}
