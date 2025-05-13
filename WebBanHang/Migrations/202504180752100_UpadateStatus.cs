namespace WebBanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpadateStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_order", "Status", c => c.Int(nullable: false, defaultValue: 0)); // 0 là giá trị mặc định, ví dụ như "Pending"
        }

        public override void Down()
        {
            DropColumn("dbo.tb_order", "Status");
        }
    }
}
