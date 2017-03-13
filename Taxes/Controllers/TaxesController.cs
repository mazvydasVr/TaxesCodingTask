using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Taxes.Models;
using Taxes.Models.DbContexts;
using ViewModel = Taxes.ViewModels;

namespace Taxes.Controllers {
    [RoutePrefix("api/taxes")]
    public class TaxesController : ApiController {
        private readonly TaxesDbContext _ctx;

        public TaxesController() : this(new TaxesDbContext()) { }

        public TaxesController(TaxesDbContext ctx) {
            _ctx = ctx;
        }

        [Route("{municipality}", Name = "GetTaxes")]
        [HttpGet]
        [ResponseType(typeof(ViewModel.TaxGet))]
        public async Task<IHttpActionResult> Get(string municipality, DateTime date) {
            if(!await _ctx.Municipalities.AnyAsync(m => m.Name == municipality))
                return BadRequest("Nerastas miestas/savivaldybė");

            var tax = await _ctx.Taxes.Where(t => t.DateFrom <= date && t.DateTo >= date)
                          .Where(t => t.Municipality == municipality)
                          .Select(t => new {
                              Tax = t,
                              dateDiff = DbFunctions.DiffDays(t.DateFrom, t.DateTo)
                          }).OrderBy(o => o.dateDiff)
                          .ThenByDescending(o => o.Tax.Created)
                          .Select(o => new ViewModel.TaxGet {
                              Id = o.Tax.Id,
                              Municipality = o.Tax.Municipality,
                              DateFrom = o.Tax.DateFrom,
                              DateTo = o.Tax.DateTo,
                              Rate = o.Tax.Rate
                          })
                          .FirstOrDefaultAsync();
            if(tax != null)
                return Ok(tax);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{municipalityName}")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(string municipalityName, [FromBody] ViewModel.TaxPost taxPost) {
            if(taxPost.DateTo != null && taxPost.DateFrom > taxPost.DateTo)
                return BadRequest("Data nuo turi būti didesnė už datą iki");

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var municipality = await _ctx.Municipalities.Include(m => m.Taxes)
                                         .SingleOrDefaultAsync(m => m.Name == municipalityName);
            if(municipality == null)
                return BadRequest("Nerastas miestas/savivaldybė");

            var newTax = new Tax(municipalityName, taxPost.DateFrom.Value, taxPost.DateTo, taxPost.Rate.Value);
            municipality.Taxes.Add(newTax);

            await _ctx.SaveChangesAsync();

            return Created(new Uri(Url.Link("GetTaxes", new {
                municipality = municipalityName,
                date = newTax.DateFrom
            })), newTax.Id);
        }
        

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if(disposing) {
                _ctx?.Dispose();
            }
        }
    }
}