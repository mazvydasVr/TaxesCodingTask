using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Moq;
using Taxes.Controllers;
using Taxes.Models;
using Taxes.Models.DbContexts;
using Xunit;
using ViewModel = Taxes.ViewModels;

namespace Taxes.Tests.Controllers {

    [Collection(TestsGlobalFixture.CollectionName)]
    public class TaxesControllerTests : TestBase {
        [Fact]
        public async void POST_Tax_successfully_adds_to_municipality() {
            var urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<object>()))
                         .Returns("http://testas.lt/");

            using(var ctx = new TaxesDbContext()) {
                ctx.Municipalities.Add(new Municipality("Vilnius"));
                await ctx.SaveChangesAsync();

                using(var controller = new TaxesController(ctx)) {
                    var dateFrom = new DateTime(2017, 5, 5);
                    var dateTo = new DateTime(2017, 5, 10);
                    controller.Url = urlHelperMock.Object;
                    var result = await controller.Post("Vilnius", new ViewModel.TaxPost {
                        DateFrom = dateFrom,
                        DateTo = dateTo,
                        Rate = (decimal?)0.1
                    });
                    Assert.IsType<CreatedNegotiatedContentResult<Guid>>(result);
                    var createdResult = result as CreatedNegotiatedContentResult<Guid>;

                    var newTax = await ctx.Taxes.FirstAsync(t => t.Id == createdResult.Content);

                    Assert.Equal(newTax.Municipality, "Vilnius");
                    Assert.Equal(newTax.Rate, (decimal?)0.1);
                    Assert.Equal(newTax.DateTo, dateTo);
                    Assert.Equal(newTax.DateFrom, dateFrom);
                }
            }
        }

        [Fact]
        public async void POST_Municipality_not_found_returns_bad_request() {
            using(var ctx = new TaxesDbContext()) {
                using(var controller = new TaxesController(ctx)) {
                    var date = DateTime.Now;
                    var result = await controller.Post("Vilnius", new ViewModel.TaxPost {
                        DateFrom = date,
                        DateTo = date,
                        Rate = (decimal?)0.1
                    });
                    Assert.IsType<BadRequestErrorMessageResult>(result);
                    var badRequestResult = result as BadRequestErrorMessageResult;
                    Assert.Equal("Nerastas miestas/savivaldybė", badRequestResult.Message);
                }
            }
        }

        [Fact]
        public async void GET_Date_From_greater_than_date_to_returns_bad_request() {
            using(var controller = new TaxesController(null)) {
                var dateFrom = DateTime.Now.AddDays(15);
                var dateTo = DateTime.Now;
                var result = await controller.Post("Vilnius", new ViewModel.TaxPost {
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    Rate = (decimal?)0.1
                });
                Assert.IsType<BadRequestErrorMessageResult>(result);
                var badRequestResult = result as BadRequestErrorMessageResult;
                Assert.Equal("Data nuo turi būti didesnė už datą iki", badRequestResult.Message);
            }
        }

        [Fact]
        public async void GET_Returns_no_content_if_there_are_no_taxes_in_municipality() {
            using(var ctx = new TaxesDbContext()) {
                var municipality = new Municipality("Vilnius");

                ctx.Municipalities.Add(municipality);
                await ctx.SaveChangesAsync();

                using(var controller = new TaxesController(ctx)) {
                    var result = await controller.Get("Vilnius", DateTime.Now);

                    Assert.IsType<StatusCodeResult>(result);
                    var statusCodeResult = result as StatusCodeResult;
                    Assert.Equal(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
                }
            }
        }

        [Fact]
        public async void GET_Municipality_not_found_returns_bad_request() {
            using(var ctx = new TaxesDbContext()) {
                using(var controller = new TaxesController(ctx)) {
                    var result = await controller.Get("Vilnius", DateTime.Now);
                    Assert.IsType<BadRequestErrorMessageResult>(result);
                    var badRequestResult = result as BadRequestErrorMessageResult;
                    Assert.Equal("Nerastas miestas/savivaldybė", badRequestResult.Message);
                }
            }
        }


        [Theory]
        [MemberData("ReturnTaxesBetweenDatesIndex", MemberType = typeof(TestCase))]
        public async void Returns_taxes_between_dates(DateTime dt, decimal expected) {

            using(var ctx = new TaxesDbContext()) {
                
                var municipality = new Municipality("Vilnius");

                var yearly = new Tax("Vilnius", new DateTime(2016, 1 ,1), new DateTime(2016, 12, 31), (decimal)0.2);
                var mothly = new Tax("Vilnius", new DateTime(2016, 5, 1), new DateTime(2016, 5, 31), (decimal)0.4);
                var daily1 = new Tax("Vilnius", new DateTime(2016, 1, 1), (decimal)0.1);
                var daily2 = new Tax("Vilnius", new DateTime(2016, 12, 25), (decimal)0.1);
                municipality.AddTax(yearly);
                municipality.AddTax(mothly);
                municipality.AddTax(daily1);
                municipality.AddTax(daily2);

                ctx.Municipalities.Add(municipality);
                await ctx.SaveChangesAsync();

                using(var controller = new TaxesController(ctx)) {
                    var result = await controller.Get("Vilnius", dt);

                    Assert.IsType<OkNegotiatedContentResult<ViewModel.TaxGet>>(result);
                    var okResult = result as OkNegotiatedContentResult<ViewModel.TaxGet>;
                    Assert.Equal(okResult.Content.Rate, expected);
                }
            }
        }

        public class TestCase {
            public static readonly List<object[]> ReturnTaxesBetweenDatesTestCase = new List<object[]> {
                new object[] {
                    new DateTime(2016, 1, 1),
                    (decimal)0.1
                },
                new object[] {
                    new DateTime(2016, 5, 2),
                    (decimal)0.4
                },
                new object[] {
                    new DateTime(2016, 7, 10),
                    (decimal)0.2
                },
                new object[] {
                    new DateTime(2016, 3, 16),
                    (decimal)0.2
                },
                 new object[] {
                    new DateTime(2016, 12, 25),
                    (decimal)0.1
                },
            };

            public static IEnumerable<object[]> ReturnTaxesBetweenDatesIndex {
                get {
                    return ReturnTaxesBetweenDatesTestCase.Select(testCase => new[] {
                        testCase[0],
                        testCase[1]
                    });
                }
            }
        }
    }
}