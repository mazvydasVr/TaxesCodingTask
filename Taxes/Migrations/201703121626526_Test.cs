namespace Taxes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Tax", name: "MunicipalityName", newName: "Municipality");
            RenameIndex(table: "dbo.Tax", name: "IX_MunicipalityName", newName: "IX_Municipality");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Tax", name: "IX_Municipality", newName: "IX_MunicipalityName");
            RenameColumn(table: "dbo.Tax", name: "Municipality", newName: "MunicipalityName");
        }
    }
}
