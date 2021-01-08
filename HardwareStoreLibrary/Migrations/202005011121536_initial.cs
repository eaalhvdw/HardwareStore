using System.Data.Entity.Migrations;

namespace HardwareStoreLibrary.Migrations
{
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TimePeriod = c.Int(nullable: false),
                        Status = c.String(),
                        TotalPrice = c.Int(nullable: false),
                        Customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .Index(t => t.Customer_Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        Email = c.String(),
                        Username = c.String(nullable: false, maxLength: 25),
                        Password = c.String(nullable: false, maxLength: 20),
                        LoginErrorMsg = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tools",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        SecurityDeposit = c.Int(nullable: false),
                        DailyPrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ToolBookings",
                c => new
                    {
                        Tool_Id = c.Int(nullable: false),
                        Booking_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tool_Id, t.Booking_Id })
                .ForeignKey("dbo.Tools", t => t.Tool_Id, cascadeDelete: true)
                .ForeignKey("dbo.Bookings", t => t.Booking_Id, cascadeDelete: true)
                .Index(t => t.Tool_Id)
                .Index(t => t.Booking_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ToolBookings", "Booking_Id", "dbo.Bookings");
            DropForeignKey("dbo.ToolBookings", "Tool_Id", "dbo.Tools");
            DropForeignKey("dbo.Bookings", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.ToolBookings", new[] { "Booking_Id" });
            DropIndex("dbo.ToolBookings", new[] { "Tool_Id" });
            DropIndex("dbo.Bookings", new[] { "Customer_Id" });
            DropTable("dbo.ToolBookings");
            DropTable("dbo.Tools");
            DropTable("dbo.Customers");
            DropTable("dbo.Bookings");
        }
    }
}
