namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateImportOrder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_ImportOrderDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductName = c.String(),
                        ProductAttributeId = c.String(),
                        Note = c.String(),
                        Quantity = c.Int(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImportOrderId = c.Int(nullable: false),
                        ProductAttributes_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_ImportOrder", t => t.ImportOrderId, cascadeDelete: true)
                .ForeignKey("dbo.tb_ProductAttributes", t => t.ProductAttributes_Id)
                .Index(t => t.ImportOrderId)
                .Index(t => t.ProductAttributes_Id);
            
            CreateTable(
                "dbo.tb_ImportOrder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TotalProducts = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_ImportOrderDetail", "ProductAttributes_Id", "dbo.tb_ProductAttributes");
            DropForeignKey("dbo.tb_ImportOrderDetail", "ImportOrderId", "dbo.tb_ImportOrder");
            DropIndex("dbo.tb_ImportOrderDetail", new[] { "ProductAttributes_Id" });
            DropIndex("dbo.tb_ImportOrderDetail", new[] { "ImportOrderId" });
            DropTable("dbo.tb_ImportOrder");
            DropTable("dbo.tb_ImportOrderDetail");
        }
    }
}
