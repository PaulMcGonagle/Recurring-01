using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;

namespace Generators.XInstances
{
    public class GeneratorXRangeTime : IGeneratorX
    {
        public IVertex Generate(XElement xRangeTime, IDictionary<string, IVertex> caches, string elementsName = null, IClock clock = null)
        {
            var start = xRangeTime.RetrieveAttributeAsLocalTime("start");
            var end = xRangeTime.RetrieveAttributeAsLocalTime("end");
            var period = Period.Between(start, end);

            var rangeTime = new RangeTime(start, period);

            rangeTime.Connect(xRangeTime.RetrieveTags(caches, elementsName));

            return rangeTime;
        }
    }
}
