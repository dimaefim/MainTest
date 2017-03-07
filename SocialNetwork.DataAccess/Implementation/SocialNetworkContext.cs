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

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<UsersInRolesEntity> UsersInRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new UserRoleConfiguration());
        }
    }
}
