namespace Spikes.Migrations.DataMigrations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssetCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("Main.Assets", "Created", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("Main.Assets", "Created");
        }
    }
}
