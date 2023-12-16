namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAttribute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_ProductAttributes", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_ProductAttributes", "Name");
        }
    }
}
