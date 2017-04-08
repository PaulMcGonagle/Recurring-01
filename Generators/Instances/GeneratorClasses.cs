using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GeneratorClasses : IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile)
        {
            var xSource = XDocument
                .Load(sourceFile);

            var generatorSource = new GeneratorSource
            {
                Xml = xSource.ToString()
            };

            yield return generatorSource;

            Utilities.ExpandReferences(xSource);

            var xGenerators = xSource
                .Elements("generators")
                .Elements("generator");

            var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

            foreach (var xGenerator in xGenerators)
            {
                var xGroups = xGenerator
                    .Elements("groups")
                    .Elements("group")
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
                    var xClasses = xGroup
                        .Elements("classes")
                        .Elements("class")
                        .ToList();

                    var xGroupSessions = xGroup
                        .Elements("sessions")
                        .Elements("session")
                        .ToList();

                    var xGroupTerms = xGroup
                        .Elements("terms")
                        .Elements("term")
                        .ToList();

                    var groupName = xGroup
                        .Attribute("name")?.Value;

                    var group = organisation
                        .Connect("group", groupName);

                    var groupTags = Utilities.RetrieveTags(xGroup)
                        .ToList();

                    group.Connect(groupTags);

                    foreach (var xClass in xClasses.Where(c => c != null))
                    {
                        var className = xClass
                            .Attribute("name")?.Value;
                        var classTag = group
                            .Connect("class", className);

                        var xClassTerms = xClass
                            .Elements("terms")
                            .Elements("term")
                            .ToList();

                        var xTerms = xClassTerms
                            .Union(xGroupTerms);

                        foreach (var xTerm in xTerms)
                        {
                            var termName = xTerm.Attribute("name")?.Value;
                            var termTag = classTag.Connect("term", termName);

                            var termRange = Utilities.RetrieveRangeDate(xTerm);

                            var xTermBreaks = xTerm
                                .Elements("breaks")
                                .Elements("break")
                                .ToList();

                            var xSchedules = xClass
                                .Elements("schedules")
                                .Elements("schedule")
                                .ToList();

                            var serials = new Serials();

                            foreach (var xSchedule in xSchedules)
                            {
                                var xWeekdays = xSchedule
                                    ?.Elements("weekdays")
                                    .Elements("weekday")
                                    .Select(w => (IsoDayOfWeek)Enum.Parse(typeof(IsoDayOfWeek), w.Value))
                                    .ToList();

                                var timeRange = Utilities.RetrieveRangeTime(xSchedule);

                                var scheduleTags = Utilities.RetrieveTags(xSchedule);

                                var byWeekdays = ByWeekdays.Create(
                                    clock: fakeClock,
                                    weekdays: xWeekdays,
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
                                        var xTermBreakRange = Utilities.RetrieveRangeDate(xTermBreak);

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

                                serial.Tags = new EdgeVertexs<ITag>(serialTags) {new EdgeTag(termTag)};

                                serials.Add(serial);
                            }

                            var @event = new Event
                            {
                                Title = organisation.Value + "." + termName + "." + groupName + "." + className,
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
