using System;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class FileEntity : IdNameEntity
    {
        public string MimeType { get; set; }
        public DateTime DateCreated { get; set; }
        public string Notes { get; set; }
        public byte[] Content { get; set; }

        public int UserSettingsId { get; set; }
        public virtual UserSettingsEntity UserSettings { get; set; }
    }
}