namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatechangeTypeFK : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_ImportOrderDetail", "ProductAttributeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ImportOrderDetail", "ProductAttributeId", c => c.String());
        }
    }
}
