namespace Spikes.Migrations.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssetRequiredUserRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("Main.Assets", "RequiredUserRole_Id", c => c.Int());
            CreateIndex("Main.Assets", "RequiredUserRole_Id");
            AddForeignKey("Main.Assets", "RequiredUserRole_Id", "Base.UserRoles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Main.Assets", "RequiredUserRole_Id", "Base.UserRoles");
            DropIndex("Main.Assets", new[] { "RequiredUserRole_Id" });
            DropColumn("Main.Assets", "RequiredUserRole_Id");
        }
    }
}
