using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;
using SocialNetwork.Models.Enums;
using SocialNetwork.Models.Models;
using System.Windows.Forms;

namespace SocialNetwork.Core.Repository
{
    public class DialogsRepository : MessageRepository, IDialogsRepository
    {
        private readonly IUsersRepository _usersRepository = NinjectBindings.Instance.Get<IUsersRepository>();

        public DialogsRepository(SocialNetworkContext context) : base (context)
        { }

        public IEnumerable<DialogsViewModel> GetAllDialogs(int user)
        {
            var dialogs = _context.Dialogs.Where(y => y.DialogUsers.Any(t => t.UserId == user))
                                          .OrderByDescending(a => a.LastMessageTime)
                                          .ToList();

            var result = dialogs.Select(item => new DialogsViewModel
            {
                Id = item.Id,
                Name = item.DialogUsers.FirstOrDefault(a => a.UserId != user).User.Name + " " +
                       item.DialogUsers.FirstOrDefault(a => a.UserId != user).User.Surname,
                Photo = Convert.ToBase64String(_usersRepository.GetUserMainPhoto(item.DialogUsers.FirstOrDefault(a => a.UserId != user).User.Login)),
                LastMessage = GetDialogLastMessage(item.Id)

            });

            return result;
        }

        public bool CheckExistenceDialog(int[] users)
        {
            return _context.Dialogs.Any(
                    item =>
                        item.DialogUsers.All(a => users.Any(b => b == a.UserId))
                        && item.DialogUsers.Count == users.Count());
        }

        public bool CreateNewDialog(int[] users)
        {
            try
            {
                var dialog = new DialogEntity
                {
                    LastMessageTime = DateTime.Now
                };

                _context.Dialogs.Add(dialog);
                _context.SaveChanges();

                foreach (var item in users)
                {
                    _context.UsersInDialogs.Add(new UsersInDialogsEntity
                    {
                        DialogId = dialog.Id,
                        UserId = item
                    });
                }

                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetDialogId(int[] users)
        {
            try
            {
                var firstOrDefault = _context.Dialogs.FirstOrDefault(
                    item => 
                        item.DialogUsers.All(a => users.Any(b => b == a.UserId))
                        && item.DialogUsers.Count == users.Count());
                return firstOrDefault?.Id ?? -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public IEnumerable<MessageViewModel> GetMessages(int[] users)
        {
            int dialog = GetDialogId(users);

            var messages = _context.Messages.Where(item => item.DialogId == dialog)
                .OrderByDescending(t => t.TimeOfSend)
                .Take(50)
                .ToList();

            messages = messages.OrderBy(t => t.TimeOfSend).ToList();

            return messages.Select(item => new MessageViewModel
            {
                Id = item.Id,
                UserId = item.UserId,
                Text = item.Text,
                TimeOfSend = item.TimeOfSend.ToString("dd.MM.yyyy HH:mm")
            });
        }

        public IEnumerable<UserMessageViewModel> GetUserMessageViewModel(int[] users)
        {
            var allUsers = _context.Users.Where(item => users.Any(b => b == item.Id)).ToList();

            return allUsers.Select(item => new UserMessageViewModel
            {
                UserId = item.Id,
                Author = item.Surname + " " + item.Name,
                Photo = Convert.ToBase64String(_usersRepository.GetUserMainPhoto(item.Login))
            });
        }

        public bool SendMessage(int[] users, string message, int user)
        {
            try
            {
                if(!CheckExistenceDialog(users))
                {
                    if(!CreateNewDialog(users))
                    {
                        return false;
                    }
                }

                int dialog = GetDialogId(users);

                _context.Messages.Add(new MessageEntity
                {
                    DialogId = dialog,
                    Text = message,
                    UserId = user,
                    TimeOfSend = DateTime.Now
                });

                var allDialog = _context.Dialogs.Find(dialog);

                allDialog.LastMessageTime = DateTime.Now;

                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<int> GetDialogUsers(int id)
        {
            return _context.Dialogs.Find(id).DialogUsers.Select(item => item.UserId);

        }

        private IEnumerable<MessageEntity> GetSortedMessages(int id)
        {
            return _context.Messages.Where(item => item.DialogId == id).OrderByDescending(t => t.TimeOfSend).ToList();
        }

        private string GetDialogLastMessage(int id)
        {
            return GetSortedMessages(id).FirstOrDefault().Text;
        }
    }
}
