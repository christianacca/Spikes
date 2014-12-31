using System.Data.Entity.Migrations;

namespace Spikes.Migrations.DataMigrations.AutoMigrations
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
                        AssetTypeId = c.Int(),
                        RequiredUserRoleId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Base.LookupItems", t => t.AssetTypeId)
                .ForeignKey("Base.UserRoles", t => t.RequiredUserRoleId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.RequiredUserRoleId);
        }
        
        public override void Down()
        {
            DropForeignKey("Main.Assets", "RequiredUserRoleId", "Base.UserRoles");
            DropForeignKey("Main.Assets", "AssetTypeId", "Base.LookupItems");
            DropIndex("Main.Assets", new[] { "RequiredUserRoleId" });
            DropIndex("Main.Assets", new[] { "AssetTypeId" });
            DropTable("Main.Assets");
        }
    }
}
