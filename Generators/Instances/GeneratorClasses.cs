using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GeneratorClasses : GenerateFromFile, IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            base.GenerateSetup(sourceFile, clock, "holidays", out XElement xGenerator, out IGeneratorSource generatorSource, out XDocument xSource, out IDictionary<string, IVertex> caches);

            yield return generatorSource;

            var xGroups = xGenerator
                .Elements("groups")
                .Elements("group")
                .ToList();

            var generatorTags = xGenerator
                .RetrieveTags(caches)
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

                var groupTags = xGroup
                    .RetrieveTags(caches)
                    .ToList();

                var groupName = groupTags
                    .RetrieveValue("name");

                var group = organisation
                    .Connect("group", groupName);

                group.Connect(groupTags);

                foreach (var xClass in xClasses)
                {
                    var classTags = xClass
                        .RetrieveTags(caches)
                        .ToList();

                    var className = classTags
                        .RetrieveValue("name");

                    var xClassTerms = xClass
                        .Elements("terms")
                        .Elements("term")
                        .ToList();

                    var classTag = new Tag("class", className);

                    var xTerms = xClassTerms;

                    foreach (var xTerm in xTerms)
                    {
                        var termTags = xTerm
                            .RetrieveTags(caches)
                            .ToList();

                        var termName = termTags
                            .RetrieveValue("name");

                        var termTag = classTag.Connect("term", termName);

                        var termRanges = xTerm
                            .Elements()
                            .RetrieveDateRanges(caches)
                            .ToList();

                        var termRange = termRanges
                            .Single();

                        var xTermBreaks = xTerm
                            .Elements("breaks")
                            .Elements()
                            .ToList();

                        var xSchedules = xClass
                            .Elements("schedules")
                            .Elements("schedule")
                            .ToList();

                        var serials = new Serials();

                        foreach (var xSchedule in xSchedules)
                        {
                            var weekdays = xSchedule
                                .RetrieveWeekdays();

                            var timeRange = xSchedule
                                .RetrieveRangeTime();

                            var byWeekdays = ByWeekdays
                                .Create(
                                    clock: clock,
                                    weekdays: weekdays,
                                    dateRange: termRange);

                            ISchedule schedule;

                            if (xTermBreaks.Count > 0)
                            {
                                var compositeSchedule = CompositeSchedule
                                    .Create(
                                        clock: clock,
                                        schedule: byWeekdays,
                                        dateRange: termRange);

                                foreach (var xTermBreak in xTermBreaks)
                                {
                                    var xTermBreakRanges = xTermBreak
                                        .RetrieveDateRanges(caches)
                                        .ToList();

                                    compositeSchedule.Breaks.AddRange(xTermBreakRanges.Select(br => new EdgeVertex<IDateRange>(br)));
                                }

                                schedule = compositeSchedule;
                            }
                            else
                            {
                                schedule = byWeekdays;
                            }

                            schedule.Connect(termTags);

                            var serial = new Serial(
                                schedule: schedule,
                                timeRange: new EdgeRangeTime(timeRange),
                                timeZoneProvider: timeZoneProvider);

                            var serialTags = termTags;

                            serial.Tags = new EdgeVertexs<ITag>(serialTags) {new EdgeVertex<ITag>(termTag)};

                            serials.Add(serial);
                        }

                        var @event = new Event
                        {
                            Title = organisation.Value + "." + termName + "." + groupName + "." + className,
                            Serials = new EdgeVertexs<ISerial>(serials),
                            Tags = new EdgeVertexs<ITag>(classTags),
                        };

                        yield return @event;
                    }
                }

                yield return organisation;
            }
        }
    }
}
