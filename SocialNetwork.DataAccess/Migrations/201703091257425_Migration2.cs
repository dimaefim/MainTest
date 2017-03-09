namespace SocialNetwork.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Users", "Passowrd");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Passowrd", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Users", "Password");
        }
    }
}
