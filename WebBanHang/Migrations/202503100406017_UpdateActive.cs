namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_category", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.tb_news", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.tb_post", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.tb_product", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_product", "IsActive");
            DropColumn("dbo.tb_post", "IsActive");
            DropColumn("dbo.tb_news", "IsActive");
            DropColumn("dbo.tb_category", "IsActive");
        }
    }
}
