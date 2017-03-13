using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class UserSettings : IdEntity
    {
        public String aboutMe { get; set; }
        public ICollection<FileEntity> Files { get; set; }
        public UserEntity User { get; set; }
    }
}
