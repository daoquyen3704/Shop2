namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateproductCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_product_category", "Alias", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.tb_product_category", "SeoTitle", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_product_category", "Icon", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_product_category", "SeoDescription", c => c.String(maxLength: 500));
            AlterColumn("dbo.tb_product_category", "SeoKeywords", c => c.String(maxLength: 250));
            DropColumn("dbo.tb_product_category", "Position");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_product_category", "Position", c => c.Int(nullable: false));
            AlterColumn("dbo.tb_product_category", "SeoKeywords", c => c.String());
            AlterColumn("dbo.tb_product_category", "SeoDescription", c => c.String());
            AlterColumn("dbo.tb_product_category", "Icon", c => c.String());
            DropColumn("dbo.tb_product_category", "SeoTitle");
            DropColumn("dbo.tb_product_category", "Alias");
        }
    }
}
