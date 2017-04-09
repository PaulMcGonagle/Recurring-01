using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GeneratorSchedule : IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile)
        {
            var xSource = XDocument
                .Load(sourceFile);

            var generatorSource = new GeneratorSource
            {
                Xml = xSource.ToString(),
                GeneratorType = "schedule"
            };

            yield return generatorSource;

            var commons = Utilities.ExpandLinks(xSource);
            Utilities.ExpandReferences(xSource);

            var xGenerators = xSource
                .Elements("generators")
                .Elements("generator")
                .ToList();

            foreach (var xGenerator in xGenerators)
            {
                var xSchedules = xGenerator
                    .Elements("schedules")
                    .Elements("schedule")
                    .ToList();

                foreach (var xSchedule in xSchedules)
                {
                    var weekdays = xSchedule.RetrieveWeekdays();

                    foreach (var weekday in weekdays)
                    {
                        var dateRanges = Utilities.RetrieveDateRanges(
                            xInput: xSchedule,
                            commons: commons);

                        foreach (var dateRange in dateRanges)
                        {
                            var byWeekday = ByWeekday
                                .Create(
                                    isoDayOfWeek: weekday,
                                    dateRange: dateRange);

                            generatorSource
                                .Schedules
                                .Add(new EdgeSchedule(byWeekday));
                        }
                    }
                }
            }
        }
    }
}