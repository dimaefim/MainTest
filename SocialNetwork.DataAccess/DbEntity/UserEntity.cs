using System;
using System.Collections.Generic;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class UserEntity : IdNameEntity
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? UserLastLoginDate { get; set; }

        public virtual ICollection<UsersInRolesEntity> UserRoles { get; set; }
        public virtual UserSettingsEntity Settings { get; set; }

        public UserEntity()
        {
            UserRoles = new List<UsersInRolesEntity>();
        }
    }
}