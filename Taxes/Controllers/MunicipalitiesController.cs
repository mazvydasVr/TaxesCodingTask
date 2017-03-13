using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Taxes.Helpers;
using Taxes.Models;
using Taxes.Models.DbContexts;
using ViewModel = Taxes.ViewModels;

namespace Taxes.Controllers {
    [RoutePrefix("api/municipalities")]
    public class MunicipalitiesController : ApiController {
        private readonly TaxesDbContext _ctx;
        private const string AcceptedMediaType = "application/vnd.excel";

        public MunicipalitiesController() : this(new TaxesDbContext()) { }

        public MunicipalitiesController(TaxesDbContext ctx) {
            _ctx = ctx;
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Upload() {
            if(Request.Content.IsMimeMultipartContent()) {
                var streamProvider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                var files = streamProvider.Contents.Select(c => new {
                    file = c.ReadAsStringAsync().Result,
                    c.Headers.ContentType.MediaType
                }).ToArray();

                if(!files.Any())
                    return BadRequest("Nepridėtas failas");

                if(files.Any(f => f.MediaType != AcceptedMediaType))
                    return BadRequest("Netinkamas failo formatas");

                var csvMunicipalities = files.Select(f => f.file.GetStringListFromCsv()).SelectMany(s => s).Distinct().Select(s => s).ToList();
                var existingMunicipalities = await _ctx.Municipalities.Where(m => csvMunicipalities.Contains(m.Name)).Select(m => m.Name)
                                                       .ToListAsync();
                csvMunicipalities.RemoveAll(c => existingMunicipalities.Contains(c));

                _ctx.Municipalities.AddRange(csvMunicipalities.Select(c => new Municipality(c)));
                await _ctx.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if(disposing) {
                _ctx?.Dispose();
            }
        }
    }
}