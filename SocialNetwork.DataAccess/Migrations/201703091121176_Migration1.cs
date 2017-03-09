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
                    Passowrd = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersInRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.UsersInRoles", "UserId", "dbo.Users");
            DropIndex("dbo.Users", "IX_UserEmailUnique");
            DropIndex("dbo.UsersInRoles", new[] { "RoleId" });
            DropIndex("dbo.UsersInRoles", new[] { "UserId" });
            DropIndex("dbo.Roles", "IX_RoleNameUnique");
            DropTable("dbo.Users");
            DropTable("dbo.UsersInRoles");
            DropTable("dbo.Roles");
        }
    }
}
