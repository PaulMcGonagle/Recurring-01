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
    public class GeneratorXByOffset : IGeneratorX
    {
        public bool TryGenerate(XElement xSchedule, IDictionary<string, IVertex> caches, out IVertex vertex, string elementsName = null)
        {
            try
            {
                var tag = xSchedule
                    .RetrieveTags(caches)
                    .Where(t => t.Ident == "type")
                    .Where(t => t.Value == "ByOffset")
                    .SingleOrDefault();

                if (tag == null)
                    throw new Exception("Could not identify tags");

                var tags = tag.Tags.Select(t => t.ToVertex);

                var initialDateValue = tags.RetrieveValue("InitialDate");
                var initialDate = Retriever.RetrieveLocalDate(initialDateValue);
                var interval = tag.Tags.Select(t => t.ToVertex).RetrieveValue("Interval");

                var dateRanges = xSchedule
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

                vertex = composite;

                vertex.Connect(xSchedule.RetrieveTags(caches, elementsName));
            }
            catch (Exception)
            {
                //todo Check exception type

                vertex = null;
                return false;
            }

            return true;
        }
    }
}
