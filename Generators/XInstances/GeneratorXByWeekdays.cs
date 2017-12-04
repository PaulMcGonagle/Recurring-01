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
    public class GeneratorXByWeekdays : IGeneratorX
    {
        public IVertex Generate(XElement xByWeekdays, IDictionary<string, IVertex> caches, string elementsName = null, IClock clock = null)
        {
            var weekdays = xByWeekdays
                .RetrieveWeekdays()
                .ToList();

            var rangeDates = xByWeekdays
                .RetrieveRangeDates(
                    caches: caches)
                .ToList();

            var compositeSchedule = new CompositeSchedule();

            foreach (var rangeDate in rangeDates)
            {
                var byOffset = new Schedule(new ByWeekdays.Builder
                    {
                        Weekdays = weekdays,
                        RangeDate = rangeDate
                    }.Build());

                compositeSchedule
                    .Inclusions
                    .Add(new EdgeSchedule(byOffset));
            }

            var schedule = new Schedule(compositeSchedule);

            schedule.Connect(xByWeekdays.RetrieveTags(caches, elementsName));

            return schedule;
        }
    }
}
