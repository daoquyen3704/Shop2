namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductss : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_product", "OriginalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tb_product", "ViewCount", c => c.Int(nullable: false));
            AlterColumn("dbo.tb_product", "ProductCode", c => c.String(maxLength: 50));
            AlterColumn("dbo.tb_product", "Image", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_product", "PriceSale", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.tb_product", "SeoTitle", c => c.String(maxLength: 250));
            DropColumn("dbo.tb_product", "CategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_product", "CategoryId", c => c.Int(nullable: false));
            AlterColumn("dbo.tb_product", "SeoTitle", c => c.String());
            AlterColumn("dbo.tb_product", "PriceSale", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.tb_product", "Image", c => c.String(maxLength: 150));
            AlterColumn("dbo.tb_product", "ProductCode", c => c.String(maxLength: 150));
            DropColumn("dbo.tb_product", "ViewCount");
            DropColumn("dbo.tb_product", "OriginalPrice");
        }
    }
}
