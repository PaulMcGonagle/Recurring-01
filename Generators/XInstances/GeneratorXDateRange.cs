using Scheduler.Persistance;
using Scheduler.Ranges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Generators.XInstances
{
    public class GeneratorXDateRange : IGeneratorX
    {
        public IVertex Generate(XElement xRangeDate)
        {
            var from = Utilities.ParseAttributeAsLocalDate(xRangeDate, "start");
            var to = Utilities.ParseAttributeAsLocalDate(xRangeDate, "end");
            var dateRange = new DateRange(from, to);

            dateRange.Connect(Utilities.RetrieveTags(xRangeDate));

            return dateRange;
        }
    }
}
