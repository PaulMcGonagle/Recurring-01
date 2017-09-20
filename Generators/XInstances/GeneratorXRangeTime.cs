using System;
using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;

namespace Generators.XInstances
{
    public class GeneratorXRangeTime : IGeneratorX
    {
        public IVertex Generate(XElement xRangeTime, IDictionary<string, IVertex> caches, string elementsName = null)
        {
            var from = xRangeTime.RetrieveAttributeAsLocalTime("start");
            var to = xRangeTime.RetrieveAttributeAsLocalTime("end");
            var period = Period.Between(from, to);

            var rangeTime = new RangeTime(from, period);

            rangeTime.Connect(xRangeTime.RetrieveTags(caches, elementsName));

            return rangeTime;
        }
    }
}
