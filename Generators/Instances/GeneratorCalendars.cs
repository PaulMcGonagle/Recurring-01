using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GeneratorCalendars : GenerateFromFile, IGenerator
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
                var compositeSchedule = new CompositeSchedule();

                var calendarTags = xCalendar
                    .RetrieveTags(caches)
                    .ToList();

                tagCalendarType
                    .Connect(calendarTags.SingleOrDefault(ct => ct.Ident == "name"));

                compositeSchedule.Connect(tagCalendarType);

                generatorSource.Schedules.Add(new EdgeVertex<ISchedule>(compositeSchedule));

                var dates = xCalendar
                    .RetrieveDates(clock, caches)
                    .ToList();

                var dateList = new DateList {Items = dates};

                dateList.Connect(calendarTags);

                yield return dateList;
            }
        }
    }
}
