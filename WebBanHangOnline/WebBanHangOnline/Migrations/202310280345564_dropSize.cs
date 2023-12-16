namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropSize : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_ProductAttributes", "Size");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_ProductAttributes", "Size", c => c.String(nullable: false));
        }
    }
}
