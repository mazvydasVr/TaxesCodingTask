using System;

namespace Taxes.ViewModels {
    public class TaxGet {
        public Guid Id { get; set; }

        public string Municipality { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public decimal Rate { get; set; }
    }
}