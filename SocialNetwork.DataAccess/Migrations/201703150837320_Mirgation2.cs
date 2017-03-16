namespace SocialNetwork.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mirgation2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Files", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Files", "Notes", c => c.String(nullable: false));
        }
    }
}
