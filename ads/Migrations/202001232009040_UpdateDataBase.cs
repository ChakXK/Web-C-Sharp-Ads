namespace ads.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDataBase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ads",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        title = c.String(maxLength: 50, unicode: false),
                        text = c.String(maxLength: 255, unicode: false),
                        prise = c.Int(nullable: false),
                        datetime = c.DateTime(),
                        idSubject = c.Int(),
                        idUser = c.String(maxLength: 128),
                        idImage = c.Int(),
                        idStatusAd = c.Int(),
                        idCity = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AdStatuses", t => t.idStatusAd)
                .ForeignKey("dbo.Cities", t => t.idCity)
                .ForeignKey("dbo.Images", t => t.idImage)
                .ForeignKey("dbo.AspNetUsers", t => t.idUser, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.idSubject, cascadeDelete: true)
                .Index(t => t.idSubject)
                .Index(t => t.idUser)
                .Index(t => t.idImage)
                .Index(t => t.idStatusAd)
                .Index(t => t.idCity);
            
            CreateTable(
                "dbo.AdStatuses",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        idAd = c.Int(nullable: false),
                        name = c.String(maxLength: 255, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Ads", t => t.idAd, cascadeDelete: true)
                .Index(t => t.idAd);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        text = c.String(maxLength: 255, unicode: false),
                        datetime = c.DateTime(),
                        isRead = c.Binary(maxLength: 1),
                        idAd = c.Int(nullable: false),
                        idSender = c.String(nullable: false, maxLength: 128),
                        idRecipient = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.idRecipient)
                .ForeignKey("dbo.AspNetUsers", t => t.idSender)
                .ForeignKey("dbo.Ads", t => t.idAd, cascadeDelete: true)
                .Index(t => t.idAd)
                .Index(t => t.idSender)
                .Index(t => t.idRecipient);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        text = c.String(maxLength: 255, unicode: false),
                        datetime = c.DateTime(),
                        isRead = c.Binary(maxLength: 1),
                        idSender = c.String(nullable: false, maxLength: 128),
                        idRecipient = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.idRecipient)
                .ForeignKey("dbo.AspNetUsers", t => t.idSender)
                .Index(t => t.idSender)
                .Index(t => t.idRecipient);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Ads", "idSubject", "dbo.Subjects");
            DropForeignKey("dbo.Messages", "idAd", "dbo.Ads");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "idSender", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "idRecipient", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "idSender", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "idRecipient", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Ads", "idUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.Images", "idAd", "dbo.Ads");
            DropForeignKey("dbo.Ads", "idImage", "dbo.Images");
            DropForeignKey("dbo.Ads", "idCity", "dbo.Cities");
            DropForeignKey("dbo.Ads", "idStatusAd", "dbo.AdStatuses");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "idRecipient" });
            DropIndex("dbo.Reviews", new[] { "idSender" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Messages", new[] { "idRecipient" });
            DropIndex("dbo.Messages", new[] { "idSender" });
            DropIndex("dbo.Messages", new[] { "idAd" });
            DropIndex("dbo.Images", new[] { "idAd" });
            DropIndex("dbo.Ads", new[] { "idCity" });
            DropIndex("dbo.Ads", new[] { "idStatusAd" });
            DropIndex("dbo.Ads", new[] { "idImage" });
            DropIndex("dbo.Ads", new[] { "idUser" });
            DropIndex("dbo.Ads", new[] { "idSubject" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Subjects");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Reviews");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Messages");
            DropTable("dbo.Images");
            DropTable("dbo.Cities");
            DropTable("dbo.AdStatuses");
            DropTable("dbo.Ads");
        }
    }
}
