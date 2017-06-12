using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.XInstances
{
    public class GeneratorXByOffset : IGeneratorX
    {
        public IVertex Generate(XElement xByOffset, IDictionary<string, IVertex> caches, string elementsName = null)
        {
            var initialDateValue = xByOffset.RetrieveValue("initialDate");
            var initialDate = Retriever.RetrieveLocalDate(initialDateValue);
            var interval = xByOffset.RetrieveValue("interval");

            var dateRanges = xByOffset
                .RetrieveDateRanges(
                    caches: caches)
                .ToList();

            var composite = new CompositeSchedule();

            foreach (var dateRange in dateRanges)
            {
                var byOffset = ByOffset
                    .Create(
                        initialDate: initialDate,
                        interval: interval,
                        range: dateRange);

                composite
                    .InclusionsEdges
                    .Add(new EdgeSchedule(byOffset));
            }

            composite.Connect(xByOffset.RetrieveTags(caches, elementsName));

            return composite;
        }
    }
}
