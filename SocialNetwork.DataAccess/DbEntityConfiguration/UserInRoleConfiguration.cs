using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class UserRoleConfiguration : EntityTypeConfiguration<UsersInRolesEntity>
    {
        public UserRoleConfiguration()
        {
            ToTable("UsersInRoles");
            HasKey(x => new { x.UserId, x.RoleId });
        }
    }
}