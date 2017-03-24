namespace test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _new : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        SubCategoryId = c.Int(nullable: false, identity: true),
                        SubCategoryName = c.String(),
                        Category_CategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.SubCategoryId)
                .ForeignKey("dbo.Categories", t => t.Category_CategoryId)
                .Index(t => t.Category_CategoryId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        userId = c.Int(nullable: false, identity: true),
                        profileid = c.Long(nullable: false),
                        firstName = c.String(),
                        email = c.String(),
                        lastName = c.String(),
                        profilepic = c.String(),
                        about = c.String(),
                        gcm_id = c.String(),
                        deviceType = c.String(),
                    })
                .PrimaryKey(t => t.userId)
                .Index(t => t.profileid, unique: true);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        eventId = c.Int(nullable: false, identity: true),
                        eventName = c.String(),
                        currentParticipant = c.Int(nullable: false),
                        eventDescription = c.String(),
                        startDate = c.String(),
                        endDate = c.String(),
                        minParticipants = c.Int(nullable: false),
                        maxParticipants = c.Int(nullable: false),
                        location_longitude = c.Double(nullable: false),
                        location_latitude = c.Double(nullable: false),
                        Gender = c.String(),
                        age_startage = c.Int(nullable: false),
                        age_endage = c.Int(nullable: false),
                        cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        subCategoryName = c.String(),
                        shortaddress = c.String(),
                        createdUser_userId = c.Int(),
                        eventCategory_SubCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.eventId)
                .ForeignKey("dbo.Users", t => t.createdUser_userId)
                .ForeignKey("dbo.SubCategories", t => t.eventCategory_SubCategoryId)
                .Index(t => t.createdUser_userId)
                .Index(t => t.eventCategory_SubCategoryId);
            
            CreateTable(
                "dbo.UserJoinedEvents",
                c => new
                    {
                        EventId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventId, t.UserId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserWatchedEvents",
                c => new
                    {
                        EventId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventId, t.UserId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserCategories",
                c => new
                    {
                        User_userId = c.Int(nullable: false),
                        Category_CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_userId, t.Category_CategoryId })
                .ForeignKey("dbo.Users", t => t.User_userId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.Category_CategoryId, cascadeDelete: true)
                .Index(t => t.User_userId)
                .Index(t => t.Category_CategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserCategories", "Category_CategoryId", "dbo.Categories");
            DropForeignKey("dbo.UserCategories", "User_userId", "dbo.Users");
            DropForeignKey("dbo.UserWatchedEvents", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserWatchedEvents", "EventId", "dbo.Events");
            DropForeignKey("dbo.UserJoinedEvents", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserJoinedEvents", "EventId", "dbo.Events");
            DropForeignKey("dbo.Events", "eventCategory_SubCategoryId", "dbo.SubCategories");
            DropForeignKey("dbo.Events", "createdUser_userId", "dbo.Users");
            DropForeignKey("dbo.SubCategories", "Category_CategoryId", "dbo.Categories");
            DropIndex("dbo.UserCategories", new[] { "Category_CategoryId" });
            DropIndex("dbo.UserCategories", new[] { "User_userId" });
            DropIndex("dbo.UserWatchedEvents", new[] { "UserId" });
            DropIndex("dbo.UserWatchedEvents", new[] { "EventId" });
            DropIndex("dbo.UserJoinedEvents", new[] { "UserId" });
            DropIndex("dbo.UserJoinedEvents", new[] { "EventId" });
            DropIndex("dbo.Events", new[] { "eventCategory_SubCategoryId" });
            DropIndex("dbo.Events", new[] { "createdUser_userId" });
            DropIndex("dbo.Users", new[] { "profileid" });
            DropIndex("dbo.SubCategories", new[] { "Category_CategoryId" });
            DropTable("dbo.UserCategories");
            DropTable("dbo.UserWatchedEvents");
            DropTable("dbo.UserJoinedEvents");
            DropTable("dbo.Events");
            DropTable("dbo.Users");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Categories");
        }
    }
}
