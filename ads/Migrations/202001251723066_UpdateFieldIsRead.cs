namespace ads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFieldIsRead : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Messages", "isRead", c => c.Boolean());
            AlterColumn("dbo.Reviews", "isRead", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reviews", "isRead", c => c.Binary(maxLength: 1));
            AlterColumn("dbo.Messages", "isRead", c => c.Binary(maxLength: 1));
        }
    }
}
