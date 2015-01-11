namespace Spikes.Migrations.DataMigrations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomUserRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("Base.UserRoles", "CustomRoleProp", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("Base.UserRoles", "CustomRoleProp");
        }
    }
}
