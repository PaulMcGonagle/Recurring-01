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
    public class GeneratorSchedule : GenerateFromFile, IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            GenerateSetup(
                generatorType: "schedule",
                sourceFile: sourceFile,
                clock: clock,
                xGenerator: out XElement xGenerator,
                generatorSource: out IGeneratorSource generatorSource,
                caches: out IDictionary <string, IVertex> caches);

            yield return generatorSource;

            var xSchedules = xGenerator
                .Elements("schedules")
                .Elements("schedule")
                .ToList();

            foreach (var xSchedule in xSchedules)
            {
                var weekdays = xSchedule
                    .RetrieveWeekdays();

                foreach (var weekday in weekdays)
                {
                    var rangeDates = xSchedule
                        .RetrieveRangeDates(
                            caches: caches);

                    foreach (var rangeDate in rangeDates)
                    {
                        var byWeekday = ByWeekday
                            .Create(
                                isoDayOfWeek: weekday,
                                rangeDate: rangeDate);

                        generatorSource
                            .Schedules
                            .Add(new EdgeSchedule(byWeekday));
                    }
                }
            }
        }
    }
}