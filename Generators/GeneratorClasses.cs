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
    public class GeneratorClasses
    {
        public static IEnumerable<IVertex> Generate(string sourceFile)
        {
            var xSource = XElement.Load(sourceFile);

            var xGenerators = xSource
                .Elements("generator");

            var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

            foreach (var xGenerator in xGenerators)
            {
                var xGroups = xGenerator
                    .Elements("groups")
                    .Elements("group")
                    .ToList();
                var xGeneratorTerms = xGenerator
                    .Elements("terms")
                    .Elements("term")
                    .ToList();
                var xGeneratorTimes = xGenerator
                    .Elements("times")
                    .Elements("time")
                    .ToList();

                var generatorTags = Utilities.RetrieveTags(xGenerator)
                    .ToList();

                var organisation = generatorTags
                    .Single(t => t.Ident == "organisation");

                var timeZoneProviderTag = generatorTags
                    .SingleOrDefault(t => t.Ident == "timeZoneProvider");

                var timeZoneProvider = timeZoneProviderTag != null
                    ? timeZoneProviderTag.Value
                    : "Europe/London";

                organisation.Connect("timeZoneProvider", timeZoneProvider);

                foreach (var xGroup in xGroups)
                {
                    var xYearClasses = xGroup
                        .Elements("classes")
                        .Elements("class")
                        .ToList();

                    var xYearSessions = xGroup
                        .Elements("sessions")
                        .Elements("session")
                        .ToList();

                    var groupName = xGroup.Attribute("name")?.Value;

                    var group = organisation.Connect("group", groupName);

                    var groupTags = Utilities.RetrieveTags(xGroup)
                        .ToList();

                    group.Connect(groupTags);

                    var xYearReferenceTerms = xGroup
                        .Elements("references")
                        .Elements("reference")
                        .Where(tr => tr.Attribute("type")?.Value == "term")
                        .ToList();

                    var xReferencedTerms = Utilities.RetrieveXReferences(xSource, xYearReferenceTerms, "term")
                        .ToList();

                    foreach (var xClass in xYearClasses.Where(c => c != null))
                    {
                        var className = xClass.Attribute("name")?.Value;
                        var classTag = group.Connect("class", className);

                        var xClassTerms = xClass
                            .Elements("terms")
                            .Elements("term")
                            .ToList();

                        var xTermsCombined = xReferencedTerms
                            .Union(xClassTerms);

                        foreach (var xTerm in xTermsCombined)
                        {
                            var termName = xTerm.Attribute("name")?.Value;
                            var termTag = classTag.Connect("term", termName);

                            var termRange = Utilities.RetrieveDateRange(xTerm);

                            var xTermBreaks = xTerm
                                .Elements("breaks")
                                .Elements("break")
                                .ToList();

                            var xClassSchedules = xClass
                                .Elements("schedules")
                                .Elements("schedule")
                                .ToList();

                            ISerials serials = new Serials();

                            foreach (var xClassSchedule in xClassSchedules)
                            {
                                var xClassScheduleWeekdays = xClassSchedule
                                    ?.Elements("weekdays")
                                    .Elements("weekday")
                                    .Select(w => (IsoDayOfWeek)Enum.Parse(typeof(IsoDayOfWeek), w.Value));

                                var sessionName = xClassSchedule?.Attribute("session")?.Value;

                                var xSession = xYearSessions
                                    .FirstOrDefault(s => s.Attribute("name")?.Value == sessionName);

                                var timeRange = Utilities.RetrieveTimeRange(xSession ?? xClassSchedule);

                                var scheduleTags = Utilities.RetrieveTags(xClassSchedule);

                                var byWeekdays = ByWeekdays.Create(
                                    clock: fakeClock,
                                    weekdays: xClassScheduleWeekdays,
                                    dateRange: termRange);

                                var termTags = Utilities.RetrieveTags(xTerm);

                                ISchedule schedule;

                                if (xTermBreaks.Count > 0)
                                {
                                    var compositeSchedule = CompositeSchedule.Create(
                                        clock: fakeClock,
                                        schedule: byWeekdays,
                                        dateRange: termRange);

                                    foreach (var xTermBreak in xTermBreaks)
                                    {
                                        var xTermBreakRange = Utilities.RetrieveDateRange(xTermBreak);

                                        compositeSchedule.Breaks.Add(new EdgeVertex<IDateRange>(xTermBreakRange));
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
                                    timeZoneProvider: timeZoneProvider);

                                var serialTags = scheduleTags
                                    .Union(generatorTags)
                                    .Union(groupTags)
                                    .Union(termTags);

                                serial.Tags = new EdgeVertexs<ITag>(serialTags);
                                serial.Tags.Add(new EdgeTag(termTag));

                                serials.Add(serial);
                            }

                            var @event = new Event
                            {
                                Title = organisation?.Value + "." + termName + "." + groupName + "." + className,
                                Serials = new EdgeVertexs<ISerial>(serials),
                                Tags = new EdgeVertexs<ITag>(groupTags),
                            };

                            yield return @event;
                        }
                    }
                }

                yield return organisation;
            }
        }
    }
}
