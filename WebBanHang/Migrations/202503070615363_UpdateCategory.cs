namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCategory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_category", "Title", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.tb_category", "SeoTitle", c => c.String(maxLength: 150));
            AlterColumn("dbo.tb_category", "SeoDescription", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_category", "SeoKeywords", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_category", "SeoKeywords", c => c.String());
            AlterColumn("dbo.tb_category", "SeoDescription", c => c.String());
            AlterColumn("dbo.tb_category", "SeoTitle", c => c.String());
            AlterColumn("dbo.tb_category", "Title", c => c.String());
        }
    }
}
