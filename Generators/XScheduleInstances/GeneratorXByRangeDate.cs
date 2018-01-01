using System.Collections.Generic;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Generators.XScheduleInstances
{
    public class GeneratorXByRangeDate : IGeneratorX
    {
        public IVertex Generate(XElement xByRangeDate, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xByRangeDate, nameof(xByRangeDate));

            var rangeDate = xByRangeDate
                .RetrieveRangeDate(
                    caches: caches);

            var byRangeDate = new Schedule(
                new ByRangeDate.Builder
                    {
                        RangeDate = rangeDate,
                    }.Build()
            );

            return byRangeDate;
        }
    }
}
