namespace Spikes.Migrations.DataMigrations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssetCreatedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("Main.Assets", "CreatedBy", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("Main.Assets", "CreatedBy");
        }
    }
}
