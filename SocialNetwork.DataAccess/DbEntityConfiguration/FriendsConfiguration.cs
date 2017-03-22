using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;
namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class FriendsConfiguration : EntityTypeConfiguration<FriendsEntity>
    {
        public FriendsConfiguration()
        {
            ToTable("Friends");

            HasKey(x => new { x.UserId, x.FriendId });

            Property(t => t.FriendId).IsRequired();
            Property(t => t.UserId).IsRequired();
            Property(t => t.IsFriends).IsRequired();
            Property(t => t.RequestDate).IsRequired();
            Property(t => t.DataConfirm).IsOptional();
        }
    }
}
