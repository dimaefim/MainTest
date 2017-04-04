using System.Collections.Generic;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class UserSettingsEntity : IdEntity
    {
        public string AboutMe { get; set; }
        public virtual ICollection<FileEntity> Files { get; set; }
        public virtual UserEntity User { get; set; }
    }
}