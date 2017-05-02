using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GeneratorOffset : GenerateFromFile, IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            GenerateSetup(
                generatorType: "offset",
                sourceFile: sourceFile,
                clock: clock,
                xGenerator: out XElement xGenerator,
                generatorSource: out IGeneratorSource generatorSource,
                caches: out IDictionary<string, IVertex> caches);

            yield return generatorSource;

            var xSchedules = xGenerator
                .Elements("schedules")
                .Elements("schedule")
                .ToList();

            foreach (var xSchedule in xSchedules)
            {
                var xByOffset = xSchedule
                    .Element("byOffset");

                var fromDate = xByOffset
                    .RetrieveAttributeAsLocalDate("initialDate");

                var increment = xByOffset
                    .RetrieveAttributeValue("interval");

                var dateRanges = xSchedule
                    .Element("rangeDates")
                    .RetrieveDateRanges(
                        caches: caches)
                    .ToList();

                foreach (var dateRange in dateRanges)
                {
                    var byOffset = ByOffset
                        .Create(
                            initialDate: fromDate,
                            interval: increment,
                            range: dateRange);

                    generatorSource
                        .Schedules
                        .Add(new EdgeSchedule(byOffset));

                    yield return byOffset;
                }
            }
        }

    }
}
