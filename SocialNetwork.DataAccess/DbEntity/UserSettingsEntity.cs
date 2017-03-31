using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class UserSettingsEntity : IdEntity
    {
        public string AboutMe { get; set; }
        public virtual ICollection<FileEntity> Files { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
