namespace Spikes.Migrations.BaseDataMigrations.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
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
                    },
                annotations: new Dictionary<string, object>
                {
                    { "CustomIdentitySeed", "True" },
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
                    },
                annotations: new Dictionary<string, object>
                {
                    { "CustomIdentitySeed", "True" },
                })
                .PrimaryKey(t => t.Id);
            
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
            DropForeignKey("Base.LookupItems", "Lookup_Id", "Base.Lookups");
            DropIndex("Base.LookupItems", new[] { "Lookup_Id" });
            DropTable("Base.UserRoles");
            DropTable("Base.Lookups",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "CustomIdentitySeed", "True" },
                });
            DropTable("Base.LookupItems",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "CustomIdentitySeed", "True" },
                });
        }
    }
}
