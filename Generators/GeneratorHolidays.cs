using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Scheduler;
using Scheduler.Persistance;

namespace Generators
{
    public class GeneratorHolidays : IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile)
        {
            var xSource = XElement
                .Load(sourceFile);

            var xGenerators = xSource
                .Elements("generator");

            foreach (var xGenerator in xGenerators)
            {
                var tagHolidayCalendar = new Tag(ident: "baseType", value: "Holidays");

                var xCalendars = xGenerator
                    .Elements("calendars")
                    .Elements("calendar")
                    .ToList();

                foreach (var xCalendar in xCalendars)
                {
                    var calendarTags = Utilities
                        .RetrieveTags(xCalendar)
                        .ToList();

                    tagHolidayCalendar
                        .Connect(calendarTags.SingleOrDefault(ct => ct.Ident == "name"));

                    var dates = Utilities
                        .RetrieveDates(xCalendar)
                        .ToList();

                    foreach (var date in dates)
                    {
                        yield return date;
                    }
                }

                yield return tagHolidayCalendar;
            }
        }
    }
}
