namespace SocialNetwork.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFriends : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Friends", "UserId", "dbo.Users");
            AddForeignKey("dbo.Friends", "UserId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Friends", "UserId", "dbo.Users");
            AddForeignKey("dbo.Friends", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
