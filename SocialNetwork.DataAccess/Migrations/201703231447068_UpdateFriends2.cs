namespace SocialNetwork.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFriends2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Friends", name: "UserId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.Friends", name: "FriendId", newName: "UserId");
            RenameColumn(table: "dbo.Friends", name: "__mig_tmp__0", newName: "FriendId");
            RenameIndex(table: "dbo.Friends", name: "IX_FriendId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.Friends", name: "IX_UserId", newName: "IX_FriendId");
            RenameIndex(table: "dbo.Friends", name: "__mig_tmp__0", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Friends", name: "IX_UserId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.Friends", name: "IX_FriendId", newName: "IX_UserId");
            RenameIndex(table: "dbo.Friends", name: "__mig_tmp__0", newName: "IX_FriendId");
            RenameColumn(table: "dbo.Friends", name: "FriendId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.Friends", name: "UserId", newName: "FriendId");
            RenameColumn(table: "dbo.Friends", name: "__mig_tmp__0", newName: "UserId");
        }
    }
}
