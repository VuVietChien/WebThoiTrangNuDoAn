namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatenoteImportOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_ImportOrder", "Note", c => c.String());
            DropColumn("dbo.tb_ImportOrderDetail", "Note");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_ImportOrderDetail", "Note", c => c.String());
            DropColumn("dbo.tb_ImportOrder", "Note");
        }
    }
}
