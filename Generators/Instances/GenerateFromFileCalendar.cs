using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Generators.XInstances;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GenerateFromFileCalendar : GenerateFromFile, IGenerator
    {
        public IEnumerable<IVertex> Generate(
            string sourceFile, 
            IClock clock)
        {
            GenerateSetup(
                generatorType: "calendars",
                sourceFile: sourceFile,
                clock: clock,
                xGenerator: out XElement xGenerator,
                generatorSource: out IGeneratorSource generatorSource,
                caches: out IDictionary<string, IVertex> caches);

            yield return generatorSource;

            var tagCalendarType = new Tag(ident: "baseType", value: "Calendar");

            yield return tagCalendarType;

            var xCalendars = xGenerator
                .Elements("calendars")
                .Elements("calendar")
                .ToList();

            foreach (var xCalendar in xCalendars)
            {
                var xSchedules = xCalendar
                    .Elements("schedules")
                    .SingleOrDefault();

                var generatorSchedule = new GeneratorXCompositeSchedule();

                var compositeSchedule = (ISchedule)generatorSchedule
                    .Generate(xSchedules, caches, null, clock);

                var calendarTags = xCalendar
                    .RetrieveTags(caches)
                    .ToList();

                tagCalendarType
                    .Connect(calendarTags.SingleOrDefault(ct => ct.Ident == "name"));

                compositeSchedule.Connect(tagCalendarType);

                generatorSource.Schedules.Add(new EdgeVertex<ISchedule>(compositeSchedule));

                var dates = compositeSchedule
                    .Generate(clock)
                    .ToList();

                var dateList = ByDateList.Create(dates);

                dateList.Connect(calendarTags);

                yield return dateList;
            }
        }
    }
}
