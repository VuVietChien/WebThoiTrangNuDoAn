namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefgproductAttribute : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_OrderDetail", "ProductId", "dbo.tb_Product");
            DropIndex("dbo.tb_OrderDetail", new[] { "ProductId" });
            AddColumn("dbo.tb_OrderDetail", "ProductAttributeId", c => c.Int(nullable: false));
            AddColumn("dbo.tb_OrderDetail", "ProductAttributes_Id", c => c.Int());
            CreateIndex("dbo.tb_OrderDetail", "ProductAttributes_Id");
            AddForeignKey("dbo.tb_OrderDetail", "ProductAttributes_Id", "dbo.tb_ProductAttributes", "Id");
            DropColumn("dbo.tb_OrderDetail", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_OrderDetail", "ProductId", c => c.Int(nullable: false));
            DropForeignKey("dbo.tb_OrderDetail", "ProductAttributes_Id", "dbo.tb_ProductAttributes");
            DropIndex("dbo.tb_OrderDetail", new[] { "ProductAttributes_Id" });
            DropColumn("dbo.tb_OrderDetail", "ProductAttributes_Id");
            DropColumn("dbo.tb_OrderDetail", "ProductAttributeId");
            CreateIndex("dbo.tb_OrderDetail", "ProductId");
            AddForeignKey("dbo.tb_OrderDetail", "ProductId", "dbo.tb_Product", "Id", cascadeDelete: true);
        }
    }
}
