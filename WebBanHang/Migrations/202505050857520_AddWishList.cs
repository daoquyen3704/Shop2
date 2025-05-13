namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWishList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_wishlist",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Username = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_wishlist", "ProductId", "dbo.tb_product");
            DropIndex("dbo.tb_wishlist", new[] { "ProductId" });
            DropTable("dbo.tb_wishlist");
        }
    }
}
