namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateQuantitySold : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Product", "QuantitySold", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_Product", "QuantitySold");
        }
    }
}
