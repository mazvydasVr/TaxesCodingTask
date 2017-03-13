using System;
using System.ComponentModel.DataAnnotations;

namespace Taxes.Models {
    public class Tax {
        [Key]
        public Guid Id { get; private set; }

        public string Municipality { get; private set; }
     
        public DateTime DateFrom { get; private set; }

        public DateTime DateTo { get; private set; }

        public decimal Rate { get; private set; }

        public DateTime Created { get; private set; }

        private Tax() {
            
        }

        public Tax(string municipality, DateTime dateFrom, DateTime? dateTo, decimal rate) {
            Set(municipality, dateFrom, dateTo, rate);
        }

        public Tax(string municipality, DateTime dateFrom, decimal rate) {
            Set(municipality, dateFrom, null, rate);
        }

        public void Set(string municipality, DateTime dateFrom, DateTime? dateTo, decimal rate) {
            if(string.IsNullOrWhiteSpace(municipality))
                throw new ArgumentNullException(nameof(municipality));
            if(dateTo == null)
                dateTo = dateFrom;
            if(dateFrom > dateTo.Value)
                throw new ArgumentException($"{nameof(dateTo)} must be greater than {nameof(dateFrom)}");
            if(rate < 0)
                throw new ArgumentException($"{nameof(rate)} must be greater than or equal to zero");
            Id = Guid.NewGuid();
            Municipality = municipality;
            DateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day);
            DateTo = new DateTime(dateTo.Value.Year, dateTo.Value.Month, dateTo.Value.Day);
            Rate = rate;

            Created = DateTime.Now;
        }

        public Tax UpdateRate(decimal rate) {
            if(rate < 0)
                throw new ArgumentException($"{nameof(rate)} must be greater than or equal to zero");
            Rate = rate;
            return this;
        }
    }
}