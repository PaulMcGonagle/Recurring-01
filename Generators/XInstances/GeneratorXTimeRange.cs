using System;
using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;

namespace Generators.XInstances
{
    public class GeneratorXTimeRange : IGeneratorX
    {
        public IVertex Generate(XElement xRangeTime, IDictionary<string, IVertex> caches, string elementsName = null)
        {
            var from = xRangeTime.RetrieveAttributeAsLocalTime("start");
            var to = xRangeTime.RetrieveAttributeAsLocalTime("end");
            var period = Period.Between(from, to);

            var timeRange = new TimeRange(from, period);

            timeRange.Connect(xRangeTime.RetrieveTags(caches, elementsName));

            return timeRange;
        }
    }
}
