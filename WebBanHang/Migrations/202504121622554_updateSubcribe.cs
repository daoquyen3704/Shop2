namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSubcribe : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_subscribe", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_subscribe", "Email", c => c.String());
        }
    }
}
