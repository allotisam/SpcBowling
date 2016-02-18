namespace SpcBowling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstDbUpdate : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Players", newName: "Player");
            RenameTable(name: "dbo.Scores", newName: "Score");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Score", newName: "Scores");
            RenameTable(name: "dbo.Player", newName: "Players");
        }
    }
}
