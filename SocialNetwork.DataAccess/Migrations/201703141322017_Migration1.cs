namespace SocialNetwork.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Password = c.String(nullable: false, maxLength: 128),
                        Email = c.String(nullable: false, maxLength: 128),
                        Surname = c.String(nullable: false, maxLength: 50),
                        Patronymic = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UserLastLoginDate = c.DateTime(),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "IX_UserEmailUnique");

            CreateTable(
                "dbo.Roles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    RoleName = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.RoleName, unique: true, name: "IX_RoleNameUnique");

            CreateTable(
                "dbo.UsersInRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.UsersSettings",
                c => new
                {
                    Id = c.Int(nullable: false),
                    aboutMe = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .Index(t => t.Id);

            CreateTable(
                "dbo.Files",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    MimeType = c.String(nullable: false),
                    DateCreated = c.DateTime(nullable: false),
                    Notes = c.String(nullable: false),
                    Content = c.Binary(nullable: false),
                    UserSettingsId = c.Int(nullable: false),
                    Name = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UsersSettings", t => t.UserSettingsId)
                .Index(t => t.UserSettingsId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersSettings", "Id", "dbo.Users");
            DropForeignKey("dbo.UsersInRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UsersInRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Files", "UserSettingsId", "dbo.UsersSettings");
            DropIndex("dbo.Roles", "IX_RoleNameUnique");
            DropIndex("dbo.UsersInRoles", new[] { "RoleId" });
            DropIndex("dbo.UsersInRoles", new[] { "UserId" });
            DropIndex("dbo.Users", "IX_UserEmailUnique");
            DropIndex("dbo.UsersSettings", new[] { "Id" });
            DropIndex("dbo.Files", new[] { "UserSettingsId" });
            DropTable("dbo.Roles");
            DropTable("dbo.UsersInRoles");
            DropTable("dbo.Users");
            DropTable("dbo.UsersSettings");
            DropTable("dbo.Files");
        }
    }
}
