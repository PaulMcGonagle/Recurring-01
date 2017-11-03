using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.XInstances
{
    public class GeneratorXByRangeDate : IGeneratorX
    {
        public IVertex Generate(XElement xByRangeDate, IDictionary<string, IVertex> caches, string elementsName = null, IClock clock = null)
        {
            if (xByRangeDate == null)
                throw new ArgumentNullException(nameof(xByRangeDate));

            var rangeDate = xByRangeDate
                .RetrieveRangeDate(
                    caches: caches);

            var byRangeDate = ByRangeDate
                .Create(
                    rangeDate: rangeDate);

            return byRangeDate;
        }
    }
}
