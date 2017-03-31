using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class DialogEntity : IdEntity
    {
        public DateTime LastMessageTime { get; set; }

        public virtual ICollection<MessageEntity> Messages { get; set; } 
        public virtual ICollection<UsersInDialogsEntity> DialogUsers { get; set; }

        public DialogEntity()
        {
            Messages = new List<MessageEntity>();
            DialogUsers = new List<UsersInDialogsEntity>();
        }
    }
}
