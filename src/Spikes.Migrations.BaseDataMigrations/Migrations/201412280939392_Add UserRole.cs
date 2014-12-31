using System.Data.Entity.Migrations;

namespace Spikes.Migrations.BaseDataMigrations.Migrations
{
    public partial class AddUserRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Base.UserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DataRoleProp = c.String(),
                        FeatureRoleProp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Base.UserRoles");
        }
    }
}
