using System;
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

            ISchedule schedule;

            if (rangeDates.Any())
            {
                var compositeSchedule = new CompositeSchedule();

                compositeSchedule
                    .Inclusions
                    .AddRange(rangeDates
                        .Select(rangeDate => new EdgeSchedule(new Schedule(new ByWeekdays.Builder
                        {
                            Weekdays = weekdays,
                            RangeDate = rangeDate
                        }.Build()))));

                schedule = new Schedule(compositeSchedule);
            }
            else
            {
                schedule = new Schedule(new ByWeekdays.Builder
                {
                    Weekdays = weekdays
                }.Build());
            }

            schedule.Connect(xByWeekdays.RetrieveTags(caches, elementsName));

            return schedule;
        }
    }
}
