using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;

namespace Generators.XInstances
{
    public class GeneratorXRangeTime : IGeneratorX
    {
        public IVertex Generate(XElement xRangeTime, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            var linkedRangeTime = UtilitiesLinks<RangeTime>
                .RetrieveAll(xRangeTime, caches)
                .SingleOrDefault();

            if (linkedRangeTime != null)
                return linkedRangeTime;

            var start = xRangeTime.RetrieveAttributeAsLocalTime("start");

            Period period;

            if (xRangeTime.HasAttribute("end"))
            {
               var  end = xRangeTime.RetrieveAttributeAsLocalTime("end");

                period = Period.Between(start, end);
            }
            else
            {
                period = xRangeTime.RetrieveAttributeAsPeriod("period");
            }
            var rangeTime = new RangeTime.Builder
            {
                Start = start,
                Period = period
            }.Build();

            rangeTime.Connect(xRangeTime.RetrieveTags(caches, elementsName));

            return rangeTime;
        }
    }
}
