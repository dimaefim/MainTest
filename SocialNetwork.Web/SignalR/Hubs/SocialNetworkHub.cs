﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Ninject;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.Web.SignalR.Models;

namespace SocialNetwork.Web.SignalR.Hubs
{
    public class SocialNetworkHub : Hub
    {
        static List<UserSignalRModel> Users = new List<UserSignalRModel>();

        public void Connect(int id)
        {
            var connectionId = Context.ConnectionId;

            if (Users.All(item => item.ConnectionId != connectionId))
            {
                Users.Add(new UserSignalRModel
                {
                    ConnectionId = connectionId,
                    DBId = id
                });
            }
        }

        public void Send(int[] users, string message, int userId)
        {
            var usersRepository = NinjectBindings.Instance.Get<IUsersRepository>();
            var user = usersRepository.GetItemById(userId);

            var userModel = new
            {
                Photo = Convert.ToBase64String(usersRepository.GetUserMainPhoto(user.Login)),
                Name = user.Surname + " " + user.Name
            };

            var dialogUsers = Users.Where(item => users.Any(a => a == item.DBId)).ToList();

            foreach(var item in dialogUsers)
            {
                if(item.DBId == userId)
                {
                    continue;
                }

                Clients.Client(item.ConnectionId).getMessage(userModel, message, users);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = Users.FirstOrDefault(item => item.ConnectionId == Context.ConnectionId);

            if(user != null)
            {
                Users.Remove(user);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
