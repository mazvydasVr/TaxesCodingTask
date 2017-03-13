using System.Data.Entity;
using Taxes.Models.DbContexts;
using Xunit;

namespace Taxes.Tests {

    public class TestsGlobalFixture {
        public const string CollectionName = "Tests global";

        public TestsGlobalFixture() {
            using(var ctx = new TaxesDbContext()) {
                try {
                    Database.SetInitializer(new DropCreateDatabaseAlways<TaxesDbContext>());
                    ctx.Database.Initialize(true);
                }
                finally {
                    Database.SetInitializer<TaxesDbContext>(null);
                }
            }
        }
    }

    [CollectionDefinition(TestsGlobalFixture.CollectionName)]
    public class DatabaseCollection : ICollectionFixture<TestsGlobalFixture> {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
