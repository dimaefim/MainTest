using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.Core.Repository
{
    public class FilesRepository : FileRepository, IFilesRepository
    {
        public FilesRepository(SocialNetworkContext context) : base (context)
        { }

        public async Task<bool> SaveNewUserAvatarAsync(UserEntity user, HttpPostedFileBase uploadImage)
        {
            try
            {
                var oldPhoto = user.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto"));

                if (oldPhoto != null)
                {
                    oldPhoto.Notes = "Photo";
                    await UpdateItemAsync(oldPhoto);
                }

                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }

                var newFile = new FileEntity
                {
                    Name = uploadImage.FileName,
                    DateCreated = DateTime.Now,
                    MimeType = "image/*",
                    Notes = "MainPhoto",
                    Content = imageData,
                    UserSettingsId = user.Settings.Id

                };

                await CreateNewItemAsync(newFile);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool SaveNewUserAvatar(UserEntity user, HttpPostedFileBase uploadImage)
        {
            try
            {
                var oldPhoto = user.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto"));

                if (oldPhoto != null)
                {
                    oldPhoto.Notes = "Photo";
                    UpdateItem(oldPhoto);
                }

                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }

                var newFile = new FileEntity
                {
                    Name = uploadImage.FileName,
                    DateCreated = DateTime.Now,
                    MimeType = "image/*",
                    Notes = "MainPhoto",
                    Content = imageData,
                    UserSettingsId = user.Settings.Id

                };

                CreateNewItem(newFile);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
