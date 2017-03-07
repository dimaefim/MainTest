using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class UserRoleConfiguration : EntityTypeConfiguration<UsersInRolesEntity>
    {
        public UserRoleConfiguration()
        {
            HasKey(x => new { x.UserId, x.RoleId });
        }
    }
}