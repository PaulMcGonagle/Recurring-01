using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Generators.XInstances
{
    public class GeneratorXDateRange : IGeneratorX
    {
        public IVertex Generate(XElement xRangeDate, IDictionary<string, IVertex> caches, string elementsName = null)
        {
            var from = xRangeDate.RetrieveAttributeAsLocalDate("start");
            var to = xRangeDate.RetrieveAttributeAsLocalDate("end");

            var dateRange = new DateRange(from, to);

            dateRange.Connect(xRangeDate.RetrieveTags(caches, elementsName));

            return dateRange;
        }
    }
}
