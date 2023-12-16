namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTypetoImportOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_ImportOrder", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_ImportOrder", "Type");
        }
    }
}
