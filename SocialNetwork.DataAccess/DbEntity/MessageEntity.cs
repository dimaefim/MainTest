using System;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class MessageEntity : IdEntity
    {
        public string Text { get; set; }
        public int UserId { get; set; }
        public DateTime TimeOfSend { get; set; }

        public virtual DialogEntity Diaolg { get; set; }
        public int DialogId { get; set; }
    }
}