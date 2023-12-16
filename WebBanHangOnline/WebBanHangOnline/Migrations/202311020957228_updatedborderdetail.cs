namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedborderdetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_OrderDetail", "Product_Id", c => c.Int());
            CreateIndex("dbo.tb_OrderDetail", "Product_Id");
            AddForeignKey("dbo.tb_OrderDetail", "Product_Id", "dbo.tb_Product", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_OrderDetail", "Product_Id", "dbo.tb_Product");
            DropIndex("dbo.tb_OrderDetail", new[] { "Product_Id" });
            DropColumn("dbo.tb_OrderDetail", "Product_Id");
        }
    }
}
