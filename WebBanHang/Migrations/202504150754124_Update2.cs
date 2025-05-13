namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_category", "Link", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_category", "Link");
        }
    }
}
