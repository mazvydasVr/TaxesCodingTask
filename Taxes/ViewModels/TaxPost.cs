using System;
using System.ComponentModel.DataAnnotations;

namespace Taxes.ViewModels {
    public class TaxPost {
        [Required(ErrorMessage = "Data nuo privaloma")]
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Koficientas negali būti neigiamas")]
        [Required(ErrorMessage = "Koeficientas privalomas")]
        public decimal? Rate { get; set; }
    }
}