namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_category", "Alias", c => c.String());
            AddColumn("dbo.tb_news", "Alias", c => c.String());
            AddColumn("dbo.tb_post", "Alias", c => c.String());
            AddColumn("dbo.tb_product", "Alias", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_product", "Alias");
            DropColumn("dbo.tb_post", "Alias");
            DropColumn("dbo.tb_news", "Alias");
            DropColumn("dbo.tb_category", "Alias");
        }
    }
}
