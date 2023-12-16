namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAlert : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_ProductAttributes", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ProductAttributes", "Name", c => c.String());
        }
    }
}
