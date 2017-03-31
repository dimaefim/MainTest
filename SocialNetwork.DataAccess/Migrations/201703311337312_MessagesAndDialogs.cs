namespace SocialNetwork.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessagesAndDialogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dialogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastMessageTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        TimeOfSend = c.DateTime(nullable: false),
                        DialogId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dialogs", t => t.DialogId)
                .Index(t => t.DialogId);

            CreateTable(
                "dbo.UsersInDialogs",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    DialogId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserId, t.DialogId })
                .ForeignKey("dbo.Dialogs", t => t.DialogId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DialogId);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersInDialogs", "UserId", "dbo.Users");
            DropForeignKey("dbo.Messages", "DialogId", "dbo.Dialogs");
            DropForeignKey("dbo.UsersInDialogs", "DialogId", "dbo.Dialogs");
            DropIndex("dbo.Messages", new[] { "DialogId" });
            DropIndex("dbo.UsersInDialogs", new[] { "DialogId" });
            DropIndex("dbo.UsersInDialogs", new[] { "UserId" });
            DropTable("dbo.Messages");
            DropTable("dbo.Dialogs");
            DropTable("dbo.UsersInDialogs");
        }
    }
}
