using System.Collections.Generic;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Generators
{
    public class GeneratorXSchedule : IGeneratorXSchedule
    {
        public IVertex Generate(XElement xSchedule, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xSchedule, nameof(xSchedule));
            Guard.AgainstNull(caches, nameof(caches));

            var rangeDate = xSchedule
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
