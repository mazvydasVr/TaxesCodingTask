using System;
using System.Collections.Generic;
using System.Linq;
using Taxes.Models;
using Taxes.Models.DbContexts;
using Xunit;

namespace Taxes.Tests.Dal {
    [Collection(TestsGlobalFixture.CollectionName)]
    public class TaxesDbContextTests : TestBase {
        [Fact]
        public void Taxes_CRUD_Works() {
            using(var ctx = new TaxesDbContext()) {
                ctx.Municipalities.AddRange(new List<Municipality> {
                    new Municipality("Vilnius"),
                    new Municipality("Kaunas")
                });
                ctx.SaveChanges();

                Assert.Empty(ctx.Taxes.ToList());

                //Create
                ctx.Taxes.AddRange(new List<Tax> {
                    new Tax("Vilnius", DateTime.Now, DateTime.Now, (decimal)0.1),
                    new Tax("Kaunas", DateTime.Now, DateTime.Now, (decimal)0.3)
                });

                ctx.SaveChanges();

                //Read
                var taxes = ctx.Taxes.ToArray();
                Assert.Equal(2, taxes.Length);

                //Delete
                ctx.Taxes.Remove(taxes.First(t => t.Municipality == "Kaunas"));
                ctx.SaveChanges();

                taxes = ctx.Taxes.ToArray();
                Assert.Equal(1, taxes.Length);

                //Update

                var tax = taxes.First();
                Assert.Equal((decimal)0.1, tax.Rate);
                tax.UpdateRate((decimal)0.5);
                ctx.SaveChanges();

                Assert.Equal((decimal)0.5, ctx.Taxes.First(t => t.Municipality == "Vilnius")
                                              .Rate);
            }
        }

        [Fact]
        public void Municipalities_CRD_Works() {
            using(var ctx = new TaxesDbContext()) {
                //Create
                ctx.Municipalities.Add(new Municipality("Panevėžys"));
                ctx.SaveChanges();

                //Read
                var municipalities = ctx.Municipalities.ToArray();
                Assert.Equal(1, municipalities.Length);

                //Delete
                ctx.Municipalities.Remove(municipalities.First());
                ctx.SaveChanges();

                Assert.Empty(ctx.Municipalities.ToArray());
            }
        }

        [Fact]
        public void Taxes_Cascade_Deletes() {
            using(var ctx = new TaxesDbContext()) {
                var municipality = new Municipality("Panevėžys");
                var taxes = new List<Tax> {
                    new Tax("Vilnius", DateTime.Now, DateTime.Now, (decimal)0.1),
                    new Tax("Kaunas", DateTime.Now, DateTime.Now, (decimal)0.3)
                };

                taxes.ForEach(t => municipality.AddTax(t));

                ctx.Municipalities.Add(municipality);
                ctx.SaveChanges();

                Assert.Equal(2, ctx.Taxes.ToArray()
                                   .Length);

                //Delete
                ctx.Municipalities.Remove(municipality);
                ctx.SaveChanges();

                Assert.Empty(ctx.Taxes.ToArray());
            }
        }
    }
}