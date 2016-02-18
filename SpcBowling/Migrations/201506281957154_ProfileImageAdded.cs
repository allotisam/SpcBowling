namespace SpcBowling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProfileImageAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "ImageData", c => c.Binary());
            AddColumn("dbo.Player", "ImageMimeType", c => c.String(maxLength:50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Player", "ImageMimeType");
            DropColumn("dbo.Player", "ImageData");
        }
    }
}
