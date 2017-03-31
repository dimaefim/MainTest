﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Models.Enums;

namespace SocialNetwork.Models.Models
{
    public class UsersViewModel : MainPageViewModel
    {
        public int Id { get; set; }
        public FriendStatusEnum Status { get; set; }
    }
}