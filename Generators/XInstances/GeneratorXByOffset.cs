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
    public class GeneratorXByOffset : IGeneratorX
    {
        public IVertex Generate(XElement xByOffset, IDictionary<string, IVertex> caches, string elementsName = null, IClock clock = null)
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
                var byOffset = new Schedule
                {
                    ScheduleInstance = new ByOffset
                    {
                        InitialDate = initialDate,
                        Interval = interval,
                        EdgeRangeDate = new EdgeRangeDate(rangeDate),

                    }
                };

                composite
                    .Inclusions
                    .Add(new EdgeSchedule(byOffset));
            }

            var schedule = new ScheduleBuilder
            {
                ScheduleInstance = composite,
            }.Build();

            schedule.Connect(xByOffset.RetrieveTags(caches, elementsName));

            return schedule;
        }
    }
}
