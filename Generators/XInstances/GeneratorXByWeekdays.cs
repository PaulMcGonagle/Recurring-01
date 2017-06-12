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

            var dateRanges = xByWeekdays
                .RetrieveDateRanges(
                    caches: caches)
                .ToList();

            var composite = new CompositeSchedule();

            foreach (var dateRange in dateRanges)
            {
                var byOffset = ByWeekdays
                    .Create(
                        weekdays: weekdays,
                        dateRange: dateRange);

                composite
                    .InclusionsEdges
                    .Add(new EdgeSchedule(byOffset));
            }

            composite.Connect(xByWeekdays.RetrieveTags(caches, elementsName));

            return composite;
        }
    }
}
