﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Core.Repository;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Account
{
    public static class UserLogin
    {
        static UserLogin()
        {
            db = new SocialNetworkUW();
        }
        public static async Task<bool> Check_Existence_User(LoginViewModel user)
        {
            IEnumerable<UserEntity> allUsers = await db.Users.GetAllItems();
            UserEntity searchedUser = allUsers.FirstOrDefault
                (
                    item =>
                    (item.Login == user.Login && item.Password == user.Password) || 
                    (item.Email == user.Login && item.Password == user.Password)
                );

            if (searchedUser == null)
            {
                return false;
            }

            return true;
        }

        private static SocialNetworkUW db;
    }
}
