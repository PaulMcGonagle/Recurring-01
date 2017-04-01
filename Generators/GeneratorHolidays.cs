using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Generators
{
    public class GeneratorHolidays : IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile)
        {
            var xSource = XDocument
                .Load(sourceFile);

            Utilities.ExpandReferences(xSource);

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

                    var calendarTags = Utilities
                        .RetrieveTags(xCalendar)
                        .ToList();

                    tagHolidayCalendar
                        .Connect(calendarTags.SingleOrDefault(ct => ct.Ident == "name"));

                    compositeSchedule.Connect(tagHolidayCalendar);

                    var dates = Utilities
                        .RetrieveDates(xCalendar)
                        .ToList();

                    var dateList = new DateList {Items = dates};

                    yield return dateList;
                }

                yield return tagHolidayCalendar;
            }
        }
    }
}
