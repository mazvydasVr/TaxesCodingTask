namespace Taxes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Date : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tax", "DateFrom", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Tax", "DateTo", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tax", "DateTo", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Tax", "DateFrom", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
