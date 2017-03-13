using Taxes.Models;

namespace Taxes.Migrations {
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Taxes.Models.DbContexts.TaxesDbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Taxes.Models.DbContexts.TaxesDbContext context) {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //context.Municipalities.AddOrUpdate(new Municipality {
            //    Name = "Vilnius"
            //}, new Municipality {
            //    Name = "Kaunas"
            //}, new Municipality {
            //    Name = "Panevėžys"
            //});

        }
    }
}
