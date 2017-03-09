using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Models.Constants;
using SocialNetwork.Models.Enums;

namespace SocialNetwork.DataAccess.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SocialNetwork.DataAccess.Implementation.SocialNetworkContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SocialNetwork.DataAccess.Implementation.SocialNetworkContext context)
        {
            context.Roles.AddOrUpdate(new RoleEntity { Id = (int)RolesEnum.Admin, RoleName = Roles.Admin });
            context.Roles.AddOrUpdate(new RoleEntity { Id = (int)RolesEnum.User, RoleName = Roles.User });

            context.Users.AddOrUpdate(new UserEntity
            {
                Login = "admin",
                Password = "admin",
                Email = "admin@gmail.com",
                Name = "»ль€",
                Surname = "ясинович",
                Patronymic = "»горевич",
                DateOfBirth = new DateTime(1992, 8, 13),
                IsDeleted = false,
                UserLastLoginDate = new DateTime(2017, 3, 4),
            });

            context.SaveChanges();

            var user = context.Users.Single(t => t.Email == "admin@gmail.com");
            context.UsersInRoles.AddOrUpdate(new UsersInRolesEntity
            {
                UserId = user.Id,
                RoleId = (int)RolesEnum.Admin
            });
            context.UsersInRoles.AddOrUpdate(new UsersInRolesEntity
            {
                UserId = user.Id,
                RoleId = (int)RolesEnum.User
            });
        }
    }
}
