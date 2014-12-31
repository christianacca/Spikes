using System.Data.Entity.Migrations;

namespace Spikes.Migrations.BaseDataMigrations.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Base.LookupItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 100),
                        Lookup_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Base.Lookups", t => t.Lookup_Id)
                .Index(t => t.Lookup_Id);
            
            CreateTable(
                "Base.Lookups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        DisplayField = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Base.LookupItems", "Lookup_Id", "Base.Lookups");
            DropIndex("Base.LookupItems", new[] { "Lookup_Id" });
            DropTable("Base.Lookups");
            DropTable("Base.LookupItems");
        }
    }
}
