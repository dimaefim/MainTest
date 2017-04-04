using System;
using System.Collections.Generic;

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
