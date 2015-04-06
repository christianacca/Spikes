namespace Spikes.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FileHeaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MediaGroupId = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        CreatedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        CustomerId = c.Int(),
                        CustomerFileTypeId = c.Int(),
                        OrderId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.LookupItems", t => t.CustomerFileTypeId)
                .Index(t => t.CustomerId)
                .Index(t => t.CustomerFileTypeId)
                .Index(t => t.OrderId);

            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlacedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Units = c.Int(nullable: false),
                        MyOrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.MyOrderId, cascadeDelete: true)
                .Index(t => t.MyOrderId);
            
            CreateTable(
                "dbo.LookupItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlacedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InvoiceLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Units = c.Int(nullable: false),
                        Invoice_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.Invoice_Id)
                .Index(t => t.Invoice_Id);
            
            CreateTable(
                "dbo.OrderNonStds",
                c => new
                    {
                        MyId = c.Int(nullable: false, identity: true),
                        PlacedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MyId);
            
            CreateTable(
                "dbo.OrderLineNonStds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Units = c.Int(nullable: false),
                        PrimaryOrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderNonStds", t => t.PrimaryOrderId, cascadeDelete: true)
                .Index(t => t.PrimaryOrderId);

            // creates view(s) that fake local tables that are actually stored in external db
            SqlResource("Spikes.EntityFramework.Migrations.201503262252365_InitialCreate_ExternalDb_Up.sql");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderLineNonStds", "PrimaryOrderId", "dbo.OrderNonStds");
            DropForeignKey("dbo.InvoiceLines", "Invoice_Id", "dbo.Invoices");
            DropForeignKey("dbo.FileHeaders", "CustomerFileTypeId", "dbo.LookupItems");
            DropForeignKey("dbo.FileHeaders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.OrderLines", "MyOrderId", "dbo.Orders");
            DropForeignKey("dbo.FileHeaders", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderLineNonStds", new[] { "PrimaryOrderId" });
            DropIndex("dbo.InvoiceLines", new[] { "Invoice_Id" });
            DropIndex("dbo.OrderLines", new[] { "MyOrderId" });
            DropIndex("dbo.FileHeaders", new[] { "OrderId" });
            DropIndex("dbo.FileHeaders", new[] { "CustomerFileTypeId" });
            DropIndex("dbo.FileHeaders", new[] { "CustomerId" });
            DropTable("dbo.OrderLineNonStds");
            DropTable("dbo.OrderNonStds");
            DropTable("dbo.InvoiceLines");
            DropTable("dbo.Invoices");
            DropTable("dbo.LookupItems");
            DropTable("dbo.OrderLines");
            DropTable("dbo.Orders");
            DropTable("dbo.FileHeaders");
            DropTable("dbo.Customers");

            // teardown views that fake local tables
            SqlResource("Spikes.EntityFramework.Migrations.201503262252365_InitialCreate_ExternalDb_Down.sql");
        }
    }
}
