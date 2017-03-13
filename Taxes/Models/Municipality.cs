using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Taxes.Models {
    public class Municipality {
        [Key]
        [StringLength(256, MinimumLength = 1)]
        public string Name { get; private set; }

        public ICollection<Tax> Taxes { get; private set; }

        private Municipality() {
            
        }
        public Municipality(string name) {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            Name = name;
        }

        public Municipality AddTax(Tax tax) {
            if(tax == null) throw new ArgumentNullException(nameof(tax));
            if(Taxes == null) Taxes = new List<Tax>();
            Taxes.Add(tax);
            return this;
        }
    }
}