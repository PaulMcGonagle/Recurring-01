using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.XScheduleInstances
{
    public class GeneratorXByWeekdays : IGeneratorX
    {
        public IVertex Generate(XElement xByWeekdays, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xByWeekdays, nameof(xByWeekdays));
            Guard.AgainstNull(caches, nameof(caches));

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
                var byWeekdays = new Schedule(new ByWeekdays.Builder
                    {
                        Weekdays = weekdays,
                        RangeDate = rangeDate
                    }.Build());

                compositeSchedule
                    .Inclusions
                    .Add(new EdgeSchedule(byWeekdays));
            }

            var schedule = new Schedule(compositeSchedule);

            schedule.Connect(xByWeekdays.RetrieveTags(caches, elementsName));

            return schedule;
        }
    }
}
