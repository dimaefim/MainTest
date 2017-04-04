using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class UsersInDialogsConfiguration : EntityTypeConfiguration<UsersInDialogsEntity>
    {
        public UsersInDialogsConfiguration()
        {
            ToTable("UsersInDialogs");
            HasKey(x => new { x.UserId, x.DialogId });
        }
    }
}