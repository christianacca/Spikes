using System.Data.Entity.Migrations;

namespace Spikes.Migrations.DataMigrations.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Main.Assets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        Reference = c.String(maxLength: 100),
                        AssetType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Base.LookupItems", t => t.AssetType_Id)
                .Index(t => t.AssetType_Id);
        }
        
        public override void Down()
        {
            DropForeignKey("Main.Assets", "AssetType_Id", "Base.LookupItems");
            DropIndex("Main.Assets", new[] { "AssetType_Id" });
            DropTable("Main.Assets");
        }
    }
}
