namespace SpcBowling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        PlayerID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        LastName = c.String(nullable: false, maxLength: 30),
                        PhoneNumber = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PlayerID);
            
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        ScoreID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        BowlingScore = c.Int(nullable: false),
                        PlayerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScoreID)
                .ForeignKey("dbo.Players", t => t.PlayerID, cascadeDelete: true)
                .Index(t => t.PlayerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Scores", "PlayerID", "dbo.Players");
            DropIndex("dbo.Scores", new[] { "PlayerID" });
            DropTable("dbo.Scores");
            DropTable("dbo.Players");
        }
    }
}
