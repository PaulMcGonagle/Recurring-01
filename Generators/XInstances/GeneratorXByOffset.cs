using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.XInstances
{
    public class GeneratorXByOffset
    {
        public bool TryGenerate(XElement xSchedule, IDictionary<string, IVertex> caches, out IVertex vertex, string elementsName = null)
        {
            try
            {
                var xByOffset = xSchedule
                    .Element("byOffset");

                if (xByOffset == null)
                    throw new Exception("No byOffset could be found");

                var initialDate = xByOffset
                    .RetrieveAttributeAsLocalDate("initialDate");

                var increment = xByOffset
                    .RetrieveAttributeValue("interval");

                var dateRanges = xSchedule
                    .Element("rangeDates")
                    .RetrieveDateRanges(
                        caches: caches)
                    .ToList();

                var composite = new CompositeSchedule();

                foreach (var dateRange in dateRanges)
                {
                    var byOffset = ByOffset
                        .Create(
                            initialDate: initialDate,
                            interval: increment,
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
