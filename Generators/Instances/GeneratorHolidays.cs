using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GeneratorHolidays : IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            var xSource = XDocument
                .Load(sourceFile);

            var generatorSource = new GeneratorSource
            {
                Xml = xSource.ToString(),
                GeneratorType = "holidays"
            };

            yield return generatorSource;

            xSource.ExpandReferences();
            var caches = xSource.ExpandLinks();

            var xGenerators = xSource
                .Elements("generators")
                .Elements("generator");

            foreach (var xGenerator in xGenerators)
            {
                var tagHolidayCalendar = new Tag(ident: "baseType", value: "Calendar");

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

                    tagHolidayCalendar
                        .Connect(calendarTags.SingleOrDefault(ct => ct.Ident == "name"));

                    compositeSchedule.Connect(tagHolidayCalendar);

                    generatorSource.Schedules.Add(new EdgeVertex<ISchedule>(compositeSchedule));

                    var dates = xCalendar
                        .RetrieveDates(caches)
                        .ToList();

                    var dateList = new DateList {Items = dates};

                    yield return dateList;
                }

                yield return tagHolidayCalendar;
            }
        }
    }
}
