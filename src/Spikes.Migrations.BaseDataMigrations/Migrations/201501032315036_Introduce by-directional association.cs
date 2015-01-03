namespace Spikes.Migrations.BaseDataMigrations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Introducebydirectionalassociation : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "Base.LookupItems", name: "Lookup_Id", newName: "LookupId");
            RenameIndex(table: "Base.LookupItems", name: "IX_Lookup_Id", newName: "IX_LookupId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "Base.LookupItems", name: "IX_LookupId", newName: "IX_Lookup_Id");
            RenameColumn(table: "Base.LookupItems", name: "LookupId", newName: "Lookup_Id");
        }
    }
}
