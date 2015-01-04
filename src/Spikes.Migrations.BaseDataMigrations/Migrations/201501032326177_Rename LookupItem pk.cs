namespace Spikes.Migrations.BaseDataMigrations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameLookupItempk : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("Base.LookupItems");
            RenameColumn(table: "Base.LookupItems", name: "Id", newName: "Key");
            AddPrimaryKey("Base.LookupItems", "Key");
        }
        
        public override void Down()
        {
            DropPrimaryKey("Base.LookupItems");
            RenameColumn(table: "Base.LookupItems", name: "Key", newName: "Id");
            AddPrimaryKey("Base.LookupItems", "Id");
        }
    }
}
