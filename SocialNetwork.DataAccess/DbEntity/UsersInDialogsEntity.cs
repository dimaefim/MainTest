using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class UsersInDialogsEntity
    {
        public int UserId { get; set; }
        public int DialogId { get; set; }
        public virtual UserEntity User { get; set; }
        public virtual DialogEntity Dialog { get; set; }
    }
}
