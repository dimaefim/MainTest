using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using Ninject;
using SocialNetwork.Core.Cache;
using SocialNetwork.Core.Dependency;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;
using SocialNetwork.Models.Enums;
using SocialNetwork.Models.Models;

namespace SocialNetwork.Core.Repository
{
    public class UsersRepository : UserRepository, IUsersRepository
    {
        private readonly IFilesRepository _filesRepository = NinjectBindings.Instance.Get<IFilesRepository>();

        public UsersRepository(SocialNetworkContext context) : base (context)
        {}

        public bool CheckExistenceUser(string login, string password)
        {
            var searchedUser = _context.Users.FirstOrDefault
                (
                    item =>
                    (item.Login == login || item.Email == login) && item.Password == password
                );

            return searchedUser != null;
        }

        public bool CheckExistenceEmailOrLogin(string login, string email)
        {
            var searchedUser = _context.Users.FirstOrDefault
                (
                    item =>
                        item.Login == login || item.Email == email
                );

            return searchedUser != null;
        }

        public bool CheckExistenceEmail(string email, string login)
        {
            var searchedUser = _context.Users.FirstOrDefault
                (
                    item =>
                        item.Email == email && item.Login != login
                );

            return searchedUser != null;
        }

        public bool AddNewUser(RegistrationViewModel user)
        {
            try
            {
                var newUser = new UserEntity
                {
                    Login = user.Login,
                    Password = user.Password,
                    Name = user.Name,
                    Surname = user.Surname,
                    Patronymic = user.Patronymic.Length == 0 ? "Undefined" : user.Patronymic,
                    Email = user.Email,
                    DateOfBirth = DateTime.Parse(user.DateOfBirth),
                    IsDeleted = false,
                    UserLastLoginDate = DateTime.Now
                };

                newUser.UserRoles.Add(new UsersInRolesEntity { RoleId = (int)RolesEnum.User, User = newUser });

                newUser.Settings = new UserSettingsEntity
                {
                    AboutMe = "",
                    User = newUser
                };

                CreateNewItem(newUser);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool UpdateCurrentUser(EditProfileViewModel user)
        {
            try
            {
                var updatedUser = GetUserByLoginOrEmail(user.Login);

                updatedUser.Name = user.Name;
                updatedUser.Surname = user.Surname;
                updatedUser.Patronymic = user.Patronymic;
                updatedUser.Email = user.Email;
                updatedUser.DateOfBirth = DateTime.Parse(user.DateOfBirth);
                updatedUser.Settings.AboutMe = user.AboutMe;

                UpdateItem(updatedUser);

                SessionCache.UpdateCurrentUser();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var updatedUser = GetUserByLoginOrEmail(SessionCache.CurrentUser.Login);
                
                updatedUser.Password = model.NewPassword;

                UpdateItem(updatedUser);

                SessionCache.UpdateCurrentUser();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public UserEntity GetUserByLoginOrEmail(string login)
        {
            var searchedUser = _context.Users.FirstOrDefault
                (
                    item =>
                        item.Login == login || item.Email == login
                );

            return searchedUser;
        }

        public UserEntity GetUserByLoginOrEmailIncluding(string login)
        {
            return _context.Users.Include(item => item.FriendUsers)
                .Include(item => item.UserFriends)
                .Include(item => item.Settings)
                .Include(item => item.Settings.Files).SingleOrDefault(item => item.Login == login);
        }

        public byte[] GetUserMainPhoto(string login)
        {
            var searchedUser = GetUserByLoginOrEmail(login);

            byte[] photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

            if (searchedUser != null)
            {
                if (searchedUser.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto")) != null)
                {
                    photo = searchedUser.Settings.Files.FirstOrDefault(item => item.Notes.Equals("MainPhoto")).Content;
                }
            }

            return photo;
        }

        public bool SaveNewCurrentUserMainPhoto(HttpPostedFileBase photo, UserEntity user)
        {
            try
            {
                var searchedUser = GetUserByLoginOrEmail(user.Login);

                if (searchedUser == null)
                {
                    return false;
                }

                if (searchedUser.Settings.Files != null)
                {
                    if (!_filesRepository.SaveNewUserAvatar(searchedUser, photo))
                    {
                        return false;
                    }

                    SessionCache.UpdateCurrentUser();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<UsersViewModel> GetAllUsers(int user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status != FriendStatusEnum.Me && item.Status != FriendStatusEnum.Friends);
        }

        public IEnumerable<UsersViewModel> GetMyFriends(int user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.Friends);
        }

        public IEnumerable<UsersViewModel> GetRequests(int user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.UserWaitAccept);
        }

        public IEnumerable<UsersViewModel> GetMyRequests(int user)
        {
            return GetAllUsersStatus(user).Where(item => item.Status == FriendStatusEnum.WaitAccept);
        }

        public string AddRequestToFriendList(int user, int id)
        {
            try
            {
                switch (GetUserStatus(user, id))
                {
                    case FriendStatusEnum.Me:

                    return "me";

                    case FriendStatusEnum.Friends:
                        var deletedFriend =
                            _context.Friends.FirstOrDefault(t => (t.FriendId == id || t.FriendId == user) &&
                                                                 (t.UserId == id || t.UserId == user) && t.IsFriends);

                        _context.Friends.Remove(deletedFriend);

                        _context.SaveChanges();

                        return "no friend";

                    case FriendStatusEnum.WaitAccept:
                        _context.Friends.Remove(
                            _context.Friends.FirstOrDefault(t => t.UserId == user && t.FriendId == id)
                            );

                        _context.SaveChanges();

                        return "no friend";

                    case FriendStatusEnum.UserWaitAccept:
                        var friend = _context.Friends.FirstOrDefault(
                            item => item.UserId == id && item.FriendId == user);

                        friend.IsFriends = true;
                        friend.DataConfirm = DateTime.Now;

                        _context.SaveChanges();

                        return "i accept";

                    case FriendStatusEnum.NoFriends:
                        _context.Friends.Add(new FriendsEntity
                        {
                            RequestDate = DateTime.Now,
                            IsFriends = false,
                            FriendId = id,
                            UserId = user
                        });

                        _context.SaveChanges();

                        return "request";
                }
            }
            catch (Exception)
            {
                return "false";
            }

            return "false";
        }

        public UsersViewModel GetUserPage(UserEntity mainUser, int id)
        {
            var user = GetItemById(id);

            var model = new UsersViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                DateOfBirth = user.DateOfBirth,
                AboutMe = user.Settings.AboutMe,
                MainPhoto = GetUserMainPhoto(user.Login),
                Status = GetUserStatus(mainUser.Id, user.Id)
            };

            return model;
        }

        public IEnumerable<DialogsViewModel> GetAllDialogs(int user)
        {
            var photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

            var dialogs = _context.Dialogs.Where(y => y.DialogUsers.Any(t => t.UserId == user))
                                          .OrderByDescending(a => a.LastMessageTime)
                                          .ToList(); //пока без ToList никак,
            //sql не знает GetDialogLastMessage, Convert.ToBase64String(), потом подумаю как убрать

            var result = dialogs.Select(item => new DialogsViewModel
            {
                Id = item.Id,
                Name = item.DialogUsers.FirstOrDefault(a => a.UserId != user).User.Name + " " +
                       item.DialogUsers.FirstOrDefault(a => a.UserId != user).User.Surname,
                Photo = item.DialogUsers.FirstOrDefault(a => a.UserId != user).User.Settings.Files.Any(i => i.Notes.Equals("MainPhoto"))
                    ? Convert.ToBase64String(item.DialogUsers.FirstOrDefault(a => a.UserId != user).User.Settings.Files.FirstOrDefault(
                        i => i.Notes.Equals("MainPhoto")).Content) 
                    : Convert.ToBase64String(photo),
                LastMessage = GetDialogLastMessage(item.Id)

            });

            return result;
        }

        public bool CheckExistenceDialog(int mainUser, int secondUser)
        {
            return _context.Dialogs.Any(item => item.DialogUsers.Any(a => a.UserId == mainUser) 
                                        && item.DialogUsers.Any(b => b.UserId == secondUser)
                                        && item.DialogUsers.Count == 2);
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

                foreach(var item in users)
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
            catch(Exception)
            {
                return false;
            }
        }

        public int GetDialogId(int[] users)
        {
            try
            {
                var firstOrDefault = _context.Dialogs
                    .FirstOrDefault(item => item.DialogUsers.All(
                        a => users.Any(b => b == a.UserId)
                             && item.DialogUsers.Count == users.Length));
                return firstOrDefault?.Id ?? -1;
            }
            catch(Exception)
            {
                return -1;
            }
        }

        public IEnumerable<MessageViewModel> GetMessages(int dialog)
        {
            var messages = _context.Messages.Where(item => item.DialogId == dialog)
                .OrderByDescending(t => t.TimeOfSend)
                .Take(50)
                .ToList();

            messages = messages.OrderBy(t => t.TimeOfSend).ToList();

            return messages.Select(item => new MessageViewModel
            {
                Id = item.Id,
                Author = GetItemById(item.UserId).Surname + " " + GetItemById(item.UserId).Name,
                Photo = Convert.ToBase64String(GetUserMainPhoto(GetItemById(item.UserId).Login)),
                Text = item.Text,
                TimeOfSend = item.TimeOfSend.ToString("dd.MM.yyyy HH:mm")
            });
        }

        public bool SendMessage(int dialog, string message, int user)
        {
            try
            {
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        private FriendStatusEnum GetUserStatus(int mainUser, int secondUser)
        {
            return mainUser == secondUser
                ? FriendStatusEnum.Me
                : _context.Friends.Any(
                    t =>
                        (t.FriendId == secondUser || t.FriendId == mainUser) &&
                        (t.UserId == secondUser || t.UserId == mainUser) && t.IsFriends)
                    ? FriendStatusEnum.Friends
                    : _context.Friends.Any(t => t.UserId == mainUser && t.FriendId == secondUser)
                        ? FriendStatusEnum.WaitAccept
                        : _context.Friends.Any(t => t.UserId == secondUser && t.FriendId == mainUser)
                            ? FriendStatusEnum.UserWaitAccept
                            : FriendStatusEnum.NoFriends;
        }

        private IEnumerable<UsersViewModel> GetAllUsersStatus(int user)
        {
            var photo = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/Home/nophoto.jpg"));

            IEnumerable<UsersViewModel> result = _context.Users.Where(y => y.Id != user).Select(item => new UsersViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth,
                AboutMe = item.Settings.AboutMe,
                MainPhoto = item.Settings.Files.Any(i => i.Notes.Equals("MainPhoto"))
                    ? item.Settings.Files.FirstOrDefault(i => i.Notes.Equals("MainPhoto")).Content :
                    photo,
                Status = user == item.Id
                ? FriendStatusEnum.Me
                : _context.Friends.Any(
                    t =>
                        (t.FriendId == item.Id || t.FriendId == user) &&
                        (t.UserId == item.Id || t.UserId == user) && t.IsFriends)
                    ? FriendStatusEnum.Friends
                    : _context.Friends.Any(t => t.UserId == user && t.FriendId == item.Id)
                        ? FriendStatusEnum.WaitAccept
                        : _context.Friends.Any(t => t.UserId == item.Id && t.FriendId == user)
                            ? FriendStatusEnum.UserWaitAccept
                            : FriendStatusEnum.NoFriends
            });

            return result;
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