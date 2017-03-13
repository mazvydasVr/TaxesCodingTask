using System;
using System.Data.Entity;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Microsoft.Owin.Testing;
using Taxes.Models;
using Taxes.Models.DbContexts;
using Taxes.ViewModels;
using Xunit;

namespace Taxes.Tests {
    [Collection(TestsGlobalFixture.CollectionName)]
    public class IntegrationTests : TestBase, IClassFixture<IntegrationTests.ClassFixture> {
        private readonly TestServer _server;

        public IntegrationTests(ClassFixture fixture) {
            _server = fixture.Server;
        }

        [Fact]
        public async void Tax_add_to_municpality_and_get_from_location() {
            using(var ctx = new TaxesDbContext()) {
                ctx.Municipalities.Add(new Municipality("Vilnius"));
                await ctx.SaveChangesAsync();
                var tax = new TaxPost {
                    DateFrom = new DateTime(2017, 1, 1),
                    DateTo = new DateTime(2017, 1, 10),
                    Rate = (decimal)0.5
                };
                //var content = new 
                var result = await _server.HttpClient.PostAsync("api/taxes/Vilnius",new ObjectContent(typeof(TaxPost), tax, new JsonMediaTypeFormatter()));
                var responseContent = await result.Content.ReadAsAsync<Guid>();

                Assert.True(result.IsSuccessStatusCode);

                var location = result.Headers.Location;
                var getResult = await _server.HttpClient.GetAsync(location);
                Assert.True(getResult.IsSuccessStatusCode);

                var savedTax = await getResult.Content.ReadAsAsync<TaxGet>();

                Assert.Equal(savedTax.Id, responseContent);
                Assert.Equal(savedTax.DateFrom, tax.DateFrom);
                Assert.Equal(savedTax.DateTo, tax.DateTo.Value);
                Assert.Equal(savedTax.Rate, tax.Rate);
            }
        }

        [Fact]
        public async void Bad_request_if_file_not_added() {
            using(var content = new MultipartFormDataContent()) {
                var result = await _server.HttpClient.PostAsync("api/municipalities", content);
                Assert.False(result.IsSuccessStatusCode);
                var response = await result.Content.ReadAsStringAsync();
                Assert.True(response.Contains("Nepridėtas failas"));
            }
        }

        [Fact]
        public async void Bad_request_if_file_format_is_wrong() {
            using(var content = new MultipartFormDataContent()) {
                using(var file = File.OpenRead(@"TestFiles\municipalitiesList.csv")) {
                    using(var stream = new StreamContent(file)) {
                        stream.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                        stream.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") {
                            FileName = "municipalitiesList.csv"
                        };
                        content.Add(stream);

                        var result = await _server.HttpClient.PostAsync("api/municipalities", content);
                        Assert.False(result.IsSuccessStatusCode);
                        var response = await result.Content.ReadAsStringAsync();
                        Assert.True(response.Contains("Netinkamas failo formatas"));
                    }
                }
            }
        }

        [Fact]
        public async void Import_file_test() {
            using(var content = new MultipartFormDataContent()) {
                using(var file = File.OpenRead(@"TestFiles\municipalitiesList.csv")) {
                    using(var stream = new StreamContent(file)) {
                        stream.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.excel");
                        stream.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") {
                            FileName = "municipalitiesList.csv"
                        };
                        content.Add(stream);

                        var result = await _server.HttpClient.PostAsync("api/municipalities", content);
                        Assert.True(result.IsSuccessStatusCode);
                    }
                }
            }
            using(var ctx = new TaxesDbContext()) {
                var municipalities = await ctx.Municipalities.ToArrayAsync();
                Assert.Equal(103, municipalities.Length);
            }
        }


        [Fact]
        public async void Import_file_adds_to_existing_test() {
            using(var ctx = new TaxesDbContext()) {
                ctx.Municipalities.Add(new Municipality("Vilnius"));
                ctx.Municipalities.Add(new Municipality("Kaunas"));
                await ctx.SaveChangesAsync();

                using(var content = new MultipartFormDataContent()) {
                    using(var file = File.OpenRead(@"TestFiles\municipalitiesList.csv")) {
                        using(var stream = new StreamContent(file)) {
                            stream.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.excel");
                            stream.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") {
                                FileName = "municipalitiesList.csv"
                            };
                            content.Add(stream);

                            var result = await _server.HttpClient.PostAsync("api/municipalities", content);
                            Assert.True(result.IsSuccessStatusCode);
                        }
                    }
                }

                var municipalities = await ctx.Municipalities.ToArrayAsync();
                Assert.Equal(103, municipalities.Length);
            }
        }

        public class ClassFixture : IDisposable {
            public readonly TestServer Server;

            public ClassFixture() {
                Server = TestServer.Create<Startup>();
            }

            public void Dispose() {
                Server?.Dispose();
            }
        }
    }
}