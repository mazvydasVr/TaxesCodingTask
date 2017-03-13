namespace Taxes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaxesDateCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tax", "Created", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tax", "Created");
        }
    }
}
