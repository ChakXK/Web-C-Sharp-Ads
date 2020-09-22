namespace ads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedChats : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "idRecipient", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "idAd", "dbo.Ads");
            DropIndex("dbo.Messages", new[] { "idAd" });
            DropIndex("dbo.Messages", new[] { "idRecipient" });
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        title = c.String(maxLength: 50, unicode: false),
                        idAd = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Ads", t => t.idAd)
                .Index(t => t.idAd);
            
            CreateTable(
                "dbo.ChatUsers",
                c => new
                    {
                        idChat = c.Int(nullable: false),
                        idUser = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.idChat, t.idUser })
                .ForeignKey("dbo.AspNetUsers", t => t.idUser)
                .ForeignKey("dbo.Chats", t => t.idChat)
                .Index(t => t.idChat)
                .Index(t => t.idUser);
            
            AddColumn("dbo.Messages", "idChat", c => c.Int(nullable: false));
            CreateIndex("dbo.Messages", "idChat");
            AddForeignKey("dbo.Messages", "idChat", "dbo.Chats", "id", cascadeDelete: true);
            DropColumn("dbo.Messages", "idAd");
            DropColumn("dbo.Messages", "idRecipient");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "idRecipient", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Messages", "idAd", c => c.Int(nullable: false));
            DropForeignKey("dbo.Chats", "idAd", "dbo.Ads");
            DropForeignKey("dbo.Messages", "idChat", "dbo.Chats");
            DropForeignKey("dbo.ChatUsers", "idChat", "dbo.Chats");
            DropForeignKey("dbo.ChatUsers", "idUser", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "idChat" });
            DropIndex("dbo.ChatUsers", new[] { "idUser" });
            DropIndex("dbo.ChatUsers", new[] { "idChat" });
            DropIndex("dbo.Chats", new[] { "idAd" });
            DropColumn("dbo.Messages", "idChat");
            DropTable("dbo.ChatUsers");
            DropTable("dbo.Chats");
            CreateIndex("dbo.Messages", "idRecipient");
            CreateIndex("dbo.Messages", "idAd");
            AddForeignKey("dbo.Messages", "idAd", "dbo.Ads", "id", cascadeDelete: true);
            AddForeignKey("dbo.Messages", "idRecipient", "dbo.AspNetUsers", "Id");
        }
    }
}
