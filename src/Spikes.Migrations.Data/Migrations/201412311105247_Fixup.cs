namespace Spikes.Migrations.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fixup : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "Main.Assets", name: "AssetType_Id", newName: "AssetTypeId");
            RenameColumn(table: "Main.Assets", name: "RequiredUserRole_Id", newName: "RequiredUserRoleId");
            RenameIndex(table: "Main.Assets", name: "IX_AssetType_Id", newName: "IX_AssetTypeId");
            RenameIndex(table: "Main.Assets", name: "IX_RequiredUserRole_Id", newName: "IX_RequiredUserRoleId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "Main.Assets", name: "IX_RequiredUserRoleId", newName: "IX_RequiredUserRole_Id");
            RenameIndex(table: "Main.Assets", name: "IX_AssetTypeId", newName: "IX_AssetType_Id");
            RenameColumn(table: "Main.Assets", name: "RequiredUserRoleId", newName: "RequiredUserRole_Id");
            RenameColumn(table: "Main.Assets", name: "AssetTypeId", newName: "AssetType_Id");
        }
    }
}
