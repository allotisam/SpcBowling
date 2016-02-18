namespace SpcBowling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HandicapScoreAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Score", "HandiScore", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Score", "HandiScore");
        }
    }
}
