using System.Data.Entity;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.DbEntityConfiguration;

namespace SocialNetwork.DataAccess.Implementation
{
    public class SocialNetworkContext : DbContext
    {
        public SocialNetworkContext() : base("DbSocialNetwork")
        {
        }

        public SocialNetworkContext(string connectionName) : base(connectionName)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<UsersInRolesEntity> UsersInRoles { get; set; }
        public DbSet<UserSettingsEntity> UsersSettings { get; set; }
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<FriendsEntity> Friends { get; set; }
        public DbSet<DialogEntity> Dialogs { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<UsersInDialogsEntity> UsersInDialogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new UserRoleConfiguration());
            modelBuilder.Configurations.Add(new UserSettingsConfiguration());
            modelBuilder.Configurations.Add(new FileConfiguration());
            modelBuilder.Configurations.Add(new FriendsConfiguration());
            modelBuilder.Configurations.Add(new MessageConfiguration());
            modelBuilder.Configurations.Add(new DialogConfiguration());
            modelBuilder.Configurations.Add(new UsersInDialogsConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
