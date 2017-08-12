using System;
using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;

namespace Generators.XInstances
{
    public class GeneratorXDateRange : IGeneratorX
    {
        public IVertex Generate(XElement xRangeDate, IDictionary<string, IVertex> caches, string elementsName = null)
        {
            var from = xRangeDate.RetrieveAttributeAsLocalDate("start");
            NodaTime.LocalDate to;

            if (xRangeDate.HasAttribute("end"))
            {
                to = xRangeDate.RetrieveAttributeAsLocalDate("end");
            }
            else
            {
                if (!xRangeDate.HasAttribute("duration"))
                    throw new Exception("Unable to generate XDateRange");

                var duration = xRangeDate.RetrieveAttributeAsTimeSpan("duration");

                //todo change to Period
                to = from.PlusDays((int)duration.TotalDays);
            }
            var dateRange = new DateRange(from, to);

            dateRange.Connect(xRangeDate.RetrieveTags(caches, elementsName));

            return dateRange;
        }
    }
}
