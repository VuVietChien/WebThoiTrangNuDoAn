namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductdb : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_Product", "OriginalPrice");
            DropColumn("dbo.tb_Product", "Price");
            DropColumn("dbo.tb_Product", "PriceSale");
            DropColumn("dbo.tb_Product", "Quantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Product", "Quantity", c => c.Int(nullable: false));
            AddColumn("dbo.tb_Product", "PriceSale", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.tb_Product", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tb_Product", "OriginalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
