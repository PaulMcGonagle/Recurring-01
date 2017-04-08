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
                    var xWeekdays = xSchedule
                        .Elements("weekdays")
                        .Elements("weekday")
                        .ToList();

                    var xRangeDates = xSchedule
                        .Elements("rangeDates")
                        .Elements("rangeDate")
                        .ToList();

                    foreach (var xWeekday in xWeekdays)
                    {
                        IsoDayOfWeek isoDayOfWeek;

                        if (!Enum.TryParse(xWeekday.Value, out isoDayOfWeek))
                            throw new Exception($"Invalid weekday '{xWeekday.Value}'");

                        var rangeDates = Utilities.RetrieveRangeDates(
                            xInput: xSchedule,
                            commons: commons);

                        foreach (var rangeDate in rangeDates)
                        {
                            var byWeekday = ByWeekday
                                .Create(
                                    isoDayOfWeek: isoDayOfWeek,
                                    dateRange: rangeDate);

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