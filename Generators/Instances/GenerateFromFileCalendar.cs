using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators.Instances
{
    public class GenerateFromFileCalendar : GenerateFromFile
    {
        public override IEnumerable<IVertex> Generate(
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

            var xCalendarsElement = xGenerator
                .Element("calendars");

            if (xCalendarsElement == null)
                throw new XmlException("Missing calendars node");

            var xCalendars = xCalendarsElement
                .Elements("calendar")
                .ToList();

            foreach (var xCalendar in xCalendars)
            {
                var calendarTags = xCalendar
                    .RetrieveTags(caches)
                    .ToList();

                foreach (var xSchedule in xCalendar
                    .Elements("schedule")
                    .Elements())
                {
                    var generatorSchedule = GenerateFromFileFactory.GetXSchedule(xSchedule.Name.LocalName);

                    var schedule = (ISchedule) generatorSchedule
                        .Generate(xSchedule, caches, clock: clock);

                    tagCalendarType
                        .Connect(calendarTags.SingleOrDefault(ct => ct.Ident == "name"));

                    schedule.Connect(tagCalendarType);

                    generatorSource.Schedules.Add(new EdgeVertex<ISchedule>(schedule));

                    schedule.Connect(calendarTags);

                    yield return schedule;
                }
            }
        }
    }
}
