namespace Taxes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Municipality",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Tax",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MunicipalityName = c.String(nullable: false, maxLength: 256),
                        DateFrom = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateTo = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Municipality", t => t.MunicipalityName, cascadeDelete: true)
                .Index(t => t.MunicipalityName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tax", "MunicipalityName", "dbo.Municipality");
            DropIndex("dbo.Tax", new[] { "MunicipalityName" });
            DropTable("dbo.Tax");
            DropTable("dbo.Municipality");
        }
    }
}
