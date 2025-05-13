namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_post", "Alias", c => c.String(maxLength: 150));
            AlterColumn("dbo.tb_post", "Image", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_post", "SeoTitle", c => c.String(maxLength: 150));
            AlterColumn("dbo.tb_post", "SeoDescription", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_post", "SeoKeywords", c => c.String(maxLength: 500));
            AlterColumn("dbo.tb_product", "Alias", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_product", "ProductCode", c => c.String(maxLength: 150));
            AlterColumn("dbo.tb_product", "Image", c => c.String(maxLength: 150));
            AlterColumn("dbo.tb_product", "SeoTitle", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_product", "SeoDescription", c => c.String(maxLength: 500));
            AlterColumn("dbo.tb_product", "SeoKeywords", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_product", "SeoKeywords", c => c.String());
            AlterColumn("dbo.tb_product", "SeoDescription", c => c.String());
            AlterColumn("dbo.tb_product", "SeoTitle", c => c.String());
            AlterColumn("dbo.tb_product", "Image", c => c.String());
            AlterColumn("dbo.tb_product", "ProductCode", c => c.String());
            AlterColumn("dbo.tb_product", "Alias", c => c.String());
            AlterColumn("dbo.tb_post", "SeoKeywords", c => c.String());
            AlterColumn("dbo.tb_post", "SeoDescription", c => c.String());
            AlterColumn("dbo.tb_post", "SeoTitle", c => c.String());
            AlterColumn("dbo.tb_post", "Image", c => c.String());
            AlterColumn("dbo.tb_post", "Alias", c => c.String());
        }
    }
}
