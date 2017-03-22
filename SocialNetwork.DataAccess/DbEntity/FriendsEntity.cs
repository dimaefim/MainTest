﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class FriendsEntity
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DataConfirm { get; set; }
        public bool IsFriends { get; set; }

        public virtual UserEntity User { get; set; }
        public virtual UserEntity Friend { get; set; }
    }
}
