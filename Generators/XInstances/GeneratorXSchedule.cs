using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Generators.XInstances
{
    public class GeneratorXSchedule : IGeneratorX
    {
        public bool TryGenerate(XElement xRangeTime, IDictionary<string, IVertex> caches, out IVertex vertex, string elementsName = null)
        {
            try
            {
                var from = xRangeTime.RetrieveAttributeAsLocalTime("start");
                var to = xRangeTime.RetrieveAttributeAsLocalTime("end");
                var period = Period.Between(from, to);

                vertex = new TimeRange(from, period);

                vertex.Connect(xRangeTime.RetrieveTags(caches, elementsName));
            }
            catch (Exception)
            {
                //todo Check exception type

                vertex = null;
                return false;
            }

            return true;
        }
    }
}
