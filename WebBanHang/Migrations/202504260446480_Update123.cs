namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update123 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_adv", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_adv", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_category", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_category", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_news", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_news", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_post", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_post", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_contact", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_contact", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_orderdetail", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_orderdetail", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_order", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_order", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_product", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_product", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_product_category", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_product_category", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_product_image", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_product_image", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_subscribe", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.tb_systemsetting", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.tb_systemsetting", "ModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_systemsetting", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_systemsetting", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_subscribe", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_product_image", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_product_image", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_product_category", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_product_category", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_product", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_product", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_order", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_order", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_orderdetail", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_orderdetail", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_contact", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_contact", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_post", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_post", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_news", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_news", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_category", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_category", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_adv", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_adv", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
