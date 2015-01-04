namespace Spikes.Migrations.DataMigrations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergeBaseModel3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Main.Assets", "AssetTypeId", "Base.LookupItems");
            RenameColumn(table: "Main.Assets", name: "AssetTypeId", newName: "AssetTypeKey");
            RenameIndex(table: "Main.Assets", name: "IX_AssetTypeId", newName: "IX_AssetTypeKey");
            DropPrimaryKey("Base.LookupItems");
            RenameColumn(table: "Base.LookupItems", name: "Id", newName: "Key");
            AddPrimaryKey("Base.LookupItems", "Key");
            AddForeignKey("Main.Assets", "AssetTypeKey", "Base.LookupItems", "Key");
        }
        
        public override void Down()
        {
            DropForeignKey("Main.Assets", "AssetTypeKey", "Base.LookupItems");
            RenameColumn(table: "Main.Assets", name: "AssetTypeKey", newName: "AssetTypeId");
            RenameIndex(table: "Main.Assets", name: "IX_AssetTypeKey", newName: "IX_AssetTypeId");
            DropPrimaryKey("Base.LookupItems");
            RenameColumn("Base.LookupItems", "Key", "Id");
            AddPrimaryKey("Base.LookupItems", "Id");
            AddForeignKey("Main.Assets", "AssetTypeId", "Base.LookupItems", "Id");
        }
    }
}
