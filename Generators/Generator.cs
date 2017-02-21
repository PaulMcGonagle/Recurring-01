using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using NodaTime.Testing;
using NodaTime.Text;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Users;

namespace Generators
{
    public class Generator
    {
        public static IEnumerable<IEvent> GenerateEvents(string sourceFile, IOrganisation organisation)
        {
            List<IEvent> events = new List<IEvent>();

            XElement xelement = XElement.Load(sourceFile);

            IEnumerable<XElement> inputGenerators = xelement
                .Elements("generator");

            var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

            foreach (var inputGenerator in inputGenerators.Where(g => g != null))
            {
                var inputGeneratorClasses = inputGenerator
                    .Elements("classes")
                    .Elements("class")
                    .ToList();
                var inputGeneratorTerms = inputGenerator
                    .Elements("terms")
                    .Elements("term")
                    .ToList();
                var inputGeneratorTimes = inputGenerator
                    .Elements("times")
                    .Elements("time")
                    .ToList();
                var inputGeneratorSessions = inputGenerator
                    .Elements("sessions")
                    .Elements("session")
                    .ToList();
                var inputGeneratorEvents = inputGenerator
                    .Elements("events")
                    .Elements("event")
                    .ToList();

                var generatorTags = RetrieveTags(inputGenerator);

                foreach (var inputEvent in inputGeneratorEvents.Where(c => c != null))
                {
                    var termName = inputEvent?.Attribute("term")?.Value;
                    var className = inputEvent?.Attribute("class")?.Value;

                    var term = inputGeneratorTerms?.SingleOrDefault(t => t.Attribute("name").Value == termName);
                    var termRange = RetrieveDateRange(term);

                    var inputClass = inputGeneratorClasses
                        .SingleOrDefault(c => c.Attribute("name")?.Value == className);

                    var title = inputClass?.Attribute("name").Value;

                    var inputSchedules = inputClass
                        ?.Elements("schedules")
                        .Elements("schedule")
                        .ToList();

                    var eventTags = RetrieveTags(inputEvent);

                    ISerials serials = new Serials();

                    foreach (var inputSchedule in inputSchedules)
                    {
                        var weekdays = inputSchedule
                            ?.Elements("weekdays")
                            .Elements("weekday")
                            .Select(w => (IsoDayOfWeek)Enum.Parse(typeof(IsoDayOfWeek), w.Value));

                        var sessionName = inputSchedule?.Attribute("session")?.Value;

                        var inputSession = inputGeneratorSessions
                            .FirstOrDefault(s => s.Attribute("name")?.Value == sessionName);

                        var timeRange = RetrieveTimeRange(inputSession ?? inputSchedule);

                        var scheduleTags = RetrieveTags(inputSchedule);

                        var byWeekdays = ByWeekdays.Create(
                            clock: fakeClock,
                            weekdays: weekdays,
                            dateRange: termRange);

                        var inputBreaks = term
                            .Elements("breaks")
                            .Elements("break")
                            .ToList();

                        var inputTermTags = RetrieveTags(term);

                        ISchedule schedule;

                        if (inputBreaks.Count > 0)
                        {
                            var compositeSchedule = CompositeSchedule.Create(
                                clock: fakeClock,
                                schedule: byWeekdays,
                                dateRange: termRange);

                            foreach (var inputBreak in inputBreaks)
                            {
                                string name = inputBreak?.Attribute("name").Value;

                                var breakRange = RetrieveDateRange(inputBreak);

                                compositeSchedule.Breaks.Add(new EdgeVertex<IDateRange>(breakRange));
                            }

                            schedule = compositeSchedule;
                        }
                        else
                        {
                            schedule = byWeekdays;
                        }

                        var serial = new Serial(
                            schedule: schedule,
                            timeRange: new EdgeRangeTime(timeRange),
                            timeZoneProvider: "Europe/London");

                        var serialTags = scheduleTags
                            .Union(eventTags)
                            .Union(inputTermTags);

                        serial.Tags = new EdgeVertexs<ITag>(serialTags);


                        serials.Add(serial);
                    }

                    var @event = new Event
                    {
                        Title = organisation.Title + "." + termName + "." + className,
                        Serials = new EdgeVertexs<ISerial>(toVertexs: serials),
                        Location = organisation.Location,
                        Tags = new EdgeVertexs<ITag>(eventTags),
                    };

                    events.Add(@event);
                }
            }

            return events;
        }


        private static DateRange RetrieveDateRange(XElement input, string elementName = "rangeDate")
        {
            var rangeDate = input.Element(elementName);

            if (rangeDate == null)
                throw new ArgumentException("Could not find Element {elementName}");

            var start = ParseAttributeAsLocalDate(rangeDate, "start");
            var end = ParseAttributeAsLocalDate(rangeDate, "end");

            return new DateRange(
                from: new EdgeDate(start),
                to: new EdgeDate(end));
        }

        private static IList<XElement> RetrieveInputTags(XElement input)
        {
            return input
                .Elements("tags")
                .Elements("tag")
                .ToList();
        }

        private static ITag RetrieveTag(XElement inputTag)
        {
            var inputRelatedTags = RetrieveInputTags(inputTag);

            var relatedTags = inputRelatedTags.Select(RetrieveTag);

            var inputIdent = ParseAttribute(inputTag, "id");
            var inputValue= ParseAttribute(inputTag, "value");
            var inputPayload = inputTag.Elements("payload").FirstOrDefault();
            
            var tag = new Tag(inputIdent.Value, inputValue.Value, inputPayload?.Value);

            tag.RelatedTags.AddRange(relatedTags.Select(relatedTag => new EdgeTag(relatedTag)));

            return tag;
        }

        private static IEnumerable<ITag> RetrieveTags(XElement input)
        {
            var inputRelatedTags = RetrieveInputTags(input);

            foreach (var inputRelatedTag in inputRelatedTags)
            {
                yield return RetrieveTag(inputRelatedTag);
            }
        }

        private static ITimeRange RetrieveTimeRange(XElement input, string elementName = "rangeTime")
        {
            var rangeTime = input.Element(elementName);

            if (rangeTime == null)
                throw new ArgumentException("Could not find Element {elementName}");

            var start = ParseAttributeAsLocalTime(rangeTime, "start");
            var end = ParseAttributeAsLocalTime(rangeTime, "end");

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
