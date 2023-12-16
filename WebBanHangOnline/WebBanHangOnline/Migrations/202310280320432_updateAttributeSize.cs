namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAttributeSize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_ProductAttributes", "Size", c => c.String(nullable: false));
            DropColumn("dbo.tb_ProductAttributes", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_ProductAttributes", "Name", c => c.String(nullable: false));
            DropColumn("dbo.tb_ProductAttributes", "Size");
        }
    }
}
