namespace SocialNetwork.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration3 : DbMigration
    {
        public override void Up()
        {
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
            DropForeignKey("dbo.Files", "UserSettingsId", "dbo.UsersSettings");
            DropIndex("dbo.UsersSettings", new[] { "Id" });
            DropIndex("dbo.Files", new[] { "UserSettingsId" });
            DropTable("dbo.UsersSettings");
            DropTable("dbo.Files");
        }
    }
}
