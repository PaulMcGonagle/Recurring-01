using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.XScheduleInstances
{
    public class GeneratorXByOffset : IGeneratorX
    {
        public IVertex Generate(XElement xByOffset, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            var initialDateValue = xByOffset.RetrieveValue("initialDate");
            var initialDate = Retriever.RetrieveLocalDate(initialDateValue);
            var interval = xByOffset.RetrieveValue("interval");

            var rangeDates = xByOffset
                .RetrieveRangeDates(
                    caches: caches)
                .ToList();

            var composite = new CompositeSchedule();

            foreach (var rangeDate in rangeDates)
            {
                var byOffset = new Schedule(new ByOffset
                    {
                        InitialDate = initialDate,
                        Interval = interval,
                        EdgeRangeDate = new EdgeRangeDate(rangeDate),

                    });

                composite
                    .Inclusions
                    .Add(new EdgeSchedule(byOffset));
            }

            var schedule = new Schedule(composite);

            schedule.Connect(xByOffset.RetrieveTags(caches, elementsName));

            return schedule;
        }
    }
}
