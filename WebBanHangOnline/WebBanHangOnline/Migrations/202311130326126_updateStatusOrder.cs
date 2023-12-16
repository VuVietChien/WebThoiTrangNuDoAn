namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateStatusOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Order", "StatusOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_Order", "StatusOrder");
        }
    }
}
