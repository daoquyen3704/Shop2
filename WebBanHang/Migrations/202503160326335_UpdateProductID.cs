namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProductID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_product", "ProductCategory_Id", "dbo.tb_product_category");
            DropIndex("dbo.tb_product", new[] { "ProductCategory_Id" });
            RenameColumn(table: "dbo.tb_product", name: "ProductCategory_Id", newName: "ProductCategoryId");
            AlterColumn("dbo.tb_product", "ProductCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.tb_product", "ProductCategoryId");
            AddForeignKey("dbo.tb_product", "ProductCategoryId", "dbo.tb_product_category", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_product", "ProductCategoryId", "dbo.tb_product_category");
            DropIndex("dbo.tb_product", new[] { "ProductCategoryId" });
            AlterColumn("dbo.tb_product", "ProductCategoryId", c => c.Int());
            RenameColumn(table: "dbo.tb_product", name: "ProductCategoryId", newName: "ProductCategory_Id");
            CreateIndex("dbo.tb_product", "ProductCategory_Id");
            AddForeignKey("dbo.tb_product", "ProductCategory_Id", "dbo.tb_product_category", "Id");
        }
    }
}
