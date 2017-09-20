using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.XInstances
{
    public class GeneratorXByWeekdays : IGeneratorX
    {
        public IVertex Generate(XElement xByWeekdays, IDictionary<string, IVertex> caches, string elementsName = null)
        {
            var weekdays = xByWeekdays.RetrieveWeekdays();

            var rangeDates = xByWeekdays
                .RetrieveRangeDates(
                    caches: caches)
                .ToList();

            var composite = new CompositeSchedule();

            foreach (var rangeDate in rangeDates)
            {
                var byOffset = ByWeekdays
                    .Create(
                        weekdays: weekdays,
                        rangeDate: rangeDate);

                composite
                    .InclusionsEdges
                    .Add(new EdgeSchedule(byOffset));
            }

            composite.Connect(xByWeekdays.RetrieveTags(caches, elementsName));

            return composite;
        }
    }
}
