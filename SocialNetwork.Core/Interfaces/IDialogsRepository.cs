using System.Collections.Generic;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Interfaces
{
    public interface IDialogsRepository
    {
        IEnumerable<DialogsViewModel> GetAllDialogs(int user);
        bool CheckExistenceDialog(int[] users);
        bool CreateNewDialog(int[] users);
        int GetDialogId(int[] users);
        IEnumerable<MessageViewModel> GetMessages(int[] users);
        IEnumerable<UserMessageViewModel> GetUserMessageViewModel(int[] users);
        bool SendMessage(int[] users, string message, int user);
        IEnumerable<int> GetDialogUsers(int id);
    }
}
