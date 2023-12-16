namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addQuantitynumnull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_ProductAttributes", "Quantity", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ProductAttributes", "Quantity", c => c.Int(nullable: false));
        }
    }
}
