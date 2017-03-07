using System.Collections.Generic;

namespace SocialNetwork.DataAccess.DbEntity
{
    public class RoleEntity : IdEntity
    {
        public string RoleName { get; set; }

        public virtual ICollection<UsersInRolesEntity> UserRoles { get; set; }

        public RoleEntity()
        {
            UserRoles = new List<UsersInRolesEntity>();
        }
    }
}
