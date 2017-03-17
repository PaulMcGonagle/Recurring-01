using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NodaTime;
using NodaTime.Testing;
using NodaTime.Text;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators
{
    public class Generator
    {
        public static IEnumerable<IEvent> GenerateEvents(string sourceFile)
        {
            var events = new List<IEvent>();

            var xSource = XElement.Load(sourceFile);

            var xGenerators = xSource
                .Elements("generator");

            var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

            foreach (var xGenerator in xGenerators)
            {
                var xYears = xGenerator
                    .Elements("years")
                    .Elements("year")
                    .ToList();
                var xGeneratorTerms = xGenerator
                    .Elements("terms")
                    .Elements("term")
                    .ToList();
                var xGeneratorTimes = xGenerator
                    .Elements("times")
                    .Elements("time")
                    .ToList();

                foreach (var xYear in xYears)
                {
                    var xClasses = xYear
                        .Elements("classes")
                        .Elements("class")
                        .ToList();

                    var xSessions = xYear
                        .Elements("sessions")
                        .Elements("session")
                        .ToList();

                    var generatorTags = RetrieveTags(xGenerator)
                        .ToList();

                    var organisation = generatorTags
                        .SingleOrDefault(t => t.Ident == "organisation");

                    var xYearReferenceTerms = xYear
                        .Elements("references")
                        .Elements("reference")
                        .Where(tr => tr.Attribute("type")?.Value == "term")
                        .ToList();

                    var xReferencedTerms = RetrieveReferences(xSource, "term", xYearReferenceTerms)
                        .ToList();

                    foreach (var xClass in xClasses.Where(c => c != null))
                    {
                        var className = xClass.Attribute("name")?.Value;

                        var xClassTerms = xClass
                            .Elements("terms")
                            .Elements("term")
                            .ToList();

                        var xTermsCombined = xReferencedTerms
                            .Union(xClassTerms);

                        foreach (var xTerm in xTermsCombined)
                        {
                            var termName = xTerm.Attribute("name")?.Value;

                            var termRange = RetrieveDateRange(xTerm);

                            var xClassSchedules = xClass
                                .Elements("schedules")
                                .Elements("schedule")
                                .ToList();

                            var yearTags = RetrieveTags(xYear)
                                .ToList();

                            ISerials serials = new Serials();

                            foreach (var xClassSchedule in xClassSchedules)
                            {
                                var xClassScheduleWeekdays = xClassSchedule
                                    ?.Elements("weekdays")
                                    .Elements("weekday")
                                    .Select(w => (IsoDayOfWeek) Enum.Parse(typeof(IsoDayOfWeek), w.Value));

                                var sessionName = xClassSchedule?.Attribute("session")?.Value;

                                var xSession = xSessions
                                    .FirstOrDefault(s => s.Attribute("name")?.Value == sessionName);

                                var timeRange = RetrieveTimeRange(xSession ?? xClassSchedule);

                                var scheduleTags = RetrieveTags(xClassSchedule);

                                var byWeekdays = ByWeekdays.Create(
                                    clock: fakeClock,
                                    weekdays: xClassScheduleWeekdays,
                                    dateRange: termRange);

                                var xTermBreaks = xTerm
                                    .Elements("breaks")
                                    .Elements("break")
                                    .ToList();

                                var termTags = RetrieveTags(xTerm);

                                ISchedule schedule;

                                if (xTermBreaks.Count > 0)
                                {
                                    var compositeSchedule = CompositeSchedule.Create(
                                        clock: fakeClock,
                                        schedule: byWeekdays,
                                        dateRange: termRange);

                                    foreach (var xTermBreak in xTermBreaks)
                                    {
                                        var xTermBreakRange = RetrieveDateRange(xTermBreak);

                                        compositeSchedule.Breaks.Add(new EdgeVertex<IDateRange>(xTermBreakRange));
                                    }

                                    schedule = compositeSchedule;
                                }
                                else
                                {
                                    schedule = byWeekdays;
                                }

                                var timeZoneProviderTag =
                                    generatorTags.SingleOrDefault(t => t.Ident == "TimeZoneProvider");

                                var timeZoneProvider = timeZoneProviderTag != null
                                    ? timeZoneProviderTag.Value
                                    : "Europe/London";

                                var serial = new Serial(
                                    schedule: schedule,
                                    timeRange: new EdgeRangeTime(timeRange),
                                    timeZoneProvider: timeZoneProvider);

                                var serialTags = scheduleTags
                                    .Union(generatorTags)
                                    .Union(yearTags)
                                    .Union(termTags);

                                serial.Tags = new EdgeVertexs<ITag>(serialTags);

                                serials.Add(serial);
                            }

                            var organisationPayload = organisation?.Payload;

                            var @event = new Event
                            {
                                Title = organisation?.Value + "." + termName + "." + className,
                                Serials = new EdgeVertexs<ISerial>(serials),
                                //Location = organisation.Location,
                                Tags = new EdgeVertexs<ITag>(yearTags),
                            };

                            events.Add(@event);
                        }
                    }
                }
            }

            return events;
        }

        private static IEnumerable<XElement> RetrieveReferences(XElement xElement, string type, List<XElement> xReferences)
        {
            var paths = xReferences
                .Where(tr => tr.Attribute("type")?.Value == type)
                .Select(tr => tr.Attribute("path")?.Value)
                .Where(p => p != null);

            foreach (var path in paths)
            {
                var item = xElement.XPathSelectElement(path);

                if (item == null)
                    throw new Exception($"Unable to load reference {path}");

                if (item.Name != type)
                    throw new Exception($"Reference {path} expected type '{type}' but found '{item.Name}'");

                yield return item;
            }
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
            var parseResult = localDatePattern.Parse(attribute?.Value);

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
            {
                throw new ArgumentNullException(name);
            }

            return attribute;
        }
    }
}
