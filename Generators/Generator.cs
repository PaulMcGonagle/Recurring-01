using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using NodaTime;
using NodaTime.Testing;
using NodaTime.Text;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Users;

namespace Generators
{
    public class Generator
    {
        public List<IEvent> GenerateEvents(string sourceFile, Organisation organisation)
        {
            List<IEvent> events = new List<IEvent>();

            XElement xelement = XElement.Load(sourceFile);

            IEnumerable<XElement> inputGenerators = xelement
                .Elements("generator");

            var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

            foreach (var inputGenerator in inputGenerators.Where(g => g != null))
            {
                var inputClasses = inputGenerator
                    .Elements("classes")
                    .Elements("class")
                    .ToList();
                var inputTerms = inputGenerator
                    .Elements("terms")
                    .Elements("term")
                    .ToList();
                var inputTimes = inputGenerator
                    .Elements("times")
                    .Elements("time")
                    .ToList();
                var inputSessions = inputGenerator
                    .Elements("sessions")
                    .Elements("session")
                    .ToList();
                var inputEvents = inputGenerator
                    .Elements("events")
                    .Elements("event")
                    .ToList();

                foreach (var inputEvent in inputEvents.Where(c => c != null))
                {
                    var termName = inputEvent?.Attribute("term")?.Value;
                    var className = inputEvent?.Attribute("class")?.Value;

                    var term = inputTerms?.SingleOrDefault(t => t.Attribute("name").Value == termName);
                    var termRange = RetreiveDateRange(term);

                    var inputClass = inputClasses
                        .SingleOrDefault(c => c.Attribute("name")?.Value == className);

                    var title = inputClass?.Attribute("name").Value;

                    var inputSchedules = inputClass
                        ?.Elements("schedules")
                        .Elements("schedule")
                        .ToList();

                    ISerials serials = new Serials();

                    foreach (var inputSchedule in inputSchedules)
                    {
                        var weekdays = inputSchedule
                            ?.Elements("weekdays")
                            .Elements("weekday")
                            .Select(w => (IsoDayOfWeek)Enum.Parse(typeof(IsoDayOfWeek), w.Value));

                        var sessionName = inputSchedule?.Attribute("session")?.Value;

                        var inputSession = inputSessions
                            .FirstOrDefault(s => s.Attribute("name")?.Value == sessionName);

                        var timeRange = RetrieveTimeRange(inputSession ?? inputSchedule);

                        var byWeekdays = ByWeekdays.Create(
                            clock: fakeClock,
                            weekdays: weekdays,
                            dateRange: termRange);

                        var inputBreaks = term
                            .Elements("breaks")
                            .Elements("break")
                            .ToList();

                        ISchedule schedule;

                        if (inputBreaks.Count > 0)
                        {
                            var compositeSchedule = CompositeSchedule.Create(
                                clock: fakeClock,
                                schedule: byWeekdays,
                                dateRange: termRange);

                            foreach (var @inputBreak in inputBreaks)
                            {
                                string name = @inputBreak?.Attribute("name").Value;

                                var breakRange = RetreiveDateRange(@inputBreak);

                                compositeSchedule.Breaks.Add(breakRange);
                            }

                            schedule = compositeSchedule;
                        }
                        else
                        {
                            schedule = byWeekdays;
                        }

                        var serial = new Serial(
                            schedule: schedule,
                            timeRange: timeRange,
                            timeZoneProvider: "Europe/London");

                        serials.Add(serial);
                    }

                    var @event = new Event
                    {
                        Title = organisation.Title + "." + termName + "." + className,
                        Serials = new EdgeVertexs<ISerial>(toVertexs: serials),
                        Location = organisation.Location,
                    };

                    events.Add(@event);
                }
            }

            return events;
        }


        private static DateRange RetreiveDateRange(XElement input, string elementName = "daterange")
        {
            var daterange = input.Element(elementName);

            if (daterange == null)
                throw new ArgumentException("Could not find Element {elementName}");

            var start = ParseAttributeAsLocalDate(daterange, "start");
            var end = ParseAttributeAsLocalDate(daterange, "end");

            return new DateRange(
                from: new EdgeDate(start),
                to: new EdgeDate(end));
        }

        private static TimeRange RetrieveTimeRange(XElement input, string elementName = "timerange")
        {
            var timerange = input.Element(elementName);

            if (timerange == null)
                throw new ArgumentException("Could not find Element {elementName}");

            var start = ParseAttributeAsLocalTime(timerange, "start");
            var end = ParseAttributeAsLocalTime(timerange, "end");

            Period period = Period.Between(start, end, PeriodUnits.AllTimeUnits);

            return new TimeRange(
                from: start, 
                period: period);
        }

        private static LocalDate ParseAttributeAsLocalDate(XElement input, string name)
        {
            var attribute = input.Attribute(name);

            var localDatePattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
            var parseResult = localDatePattern.Parse(attribute.Value);

            return parseResult.Value;
        }

        private static LocalTime ParseAttributeAsLocalTime(XElement input, string name)
        {
            var attribute = ParseAttribute(input, name);

            var localTimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
            var parseResult = localTimePattern.Parse(attribute.Value);

            return parseResult.Value;
        }

        private static XAttribute ParseAttribute(XElement input, string name)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            var attribute = input.Attribute(name);

            if (attribute == null)
                throw new ArgumentNullException(name);

            return attribute;
        }
    }
}
