namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProduct : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_product", "ProductCategoryId", "dbo.tb_product_category");
            DropIndex("dbo.tb_product", new[] { "ProductCategoryId" });
            RenameColumn(table: "dbo.tb_product", name: "ProductCategoryId", newName: "ProductCategory_Id");
            CreateTable(
                "dbo.tb_product_image",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Image = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.tb_product", "Quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.tb_product", "ProductCategory_Id", c => c.Int());
            AlterColumn("dbo.tb_product", "SeoTitle", c => c.String());
            CreateIndex("dbo.tb_product", "ProductCategory_Id");
            AddForeignKey("dbo.tb_product", "ProductCategory_Id", "dbo.tb_product_category", "Id");
            DropColumn("dbo.tb_product", "Quanlity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_product", "Quanlity", c => c.Int(nullable: false));
            DropForeignKey("dbo.tb_product", "ProductCategory_Id", "dbo.tb_product_category");
            DropForeignKey("dbo.tb_product_image", "ProductId", "dbo.tb_product");
            DropIndex("dbo.tb_product_image", new[] { "ProductId" });
            DropIndex("dbo.tb_product", new[] { "ProductCategory_Id" });
            AlterColumn("dbo.tb_product", "SeoTitle", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_product", "ProductCategory_Id", c => c.Int(nullable: false));
            DropColumn("dbo.tb_product", "Quantity");
            DropTable("dbo.tb_product_image");
            RenameColumn(table: "dbo.tb_product", name: "ProductCategory_Id", newName: "ProductCategoryId");
            CreateIndex("dbo.tb_product", "ProductCategoryId");
            AddForeignKey("dbo.tb_product", "ProductCategoryId", "dbo.tb_product_category", "Id", cascadeDelete: true);
        }
    }
}
