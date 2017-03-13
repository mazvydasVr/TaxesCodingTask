using System.Collections.Generic;
using System.Linq;

namespace Taxes.Helpers {
    public static class CsvHelper {

        public static IEnumerable<string> GetStringListFromCsv(this string csvString) {
        //Use some lib?
            return csvString.Split(',')
                               .ToList();
        }
    }
}