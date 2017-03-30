﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Enums
{
    public enum FriendStatusEnum
    {
        [Description("Статус: текущий пользователь")]
        Me = 1,
        [Description("Статус: друзья")]
        Friends = 2,
        [Description("Статус: текущий пользователь ждёт подтверждения")]
        WaitAccept = 3,
        [Description("Статус: пользователь ждёт подтверждения от текущего пользователя")]
        UserWaitAccept = 4,
        [Description("Статус: не друзья")]
        NoFriends = 5
    }
}
