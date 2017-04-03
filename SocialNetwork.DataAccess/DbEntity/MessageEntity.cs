using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
