namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSize1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_ProductAttributes", "Size", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_ProductAttributes", "Size");
        }
    }
}
