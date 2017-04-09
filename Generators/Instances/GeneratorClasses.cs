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
    public class GeneratorClasses : IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            var xSource = XDocument
                .Load(sourceFile);

            var generatorSource = new GeneratorSource
            {
                Xml = xSource.ToString()
            };

            yield return generatorSource;

            xSource.ExpandReferences();
            var commons = xSource.ExpandLinks();

            var xGenerators = xSource
                .Elements("generators")
                .Elements("generator");

            foreach (var xGenerator in xGenerators)
            {
                var xGroups = xGenerator
                    .Elements("groups")
                    .Elements("group")
                    .ToList();

                var generatorTags = xGenerator
                    .RetrieveTags(commons)
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
                        .RetrieveTags(commons)
                        .ToList();

                    var groupName = groupTags
                        .RetrieveValue("name");

                    var group = organisation
                        .Connect("group", groupName);

                    group.Connect(groupTags);

                    foreach (var xClass in xClasses)
                    {
                        var classTags = xClass
                            .RetrieveTags(commons)
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
                                .RetrieveTags(commons)
                                .ToList();

                            var termName = termTags
                                .RetrieveValue("name");

                            var termTag = classTag.Connect("term", termName);

                            var termRange = xTerm.RetrieveDateRange(commons);

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
                                var weekdays = xSchedule
                                    .RetrieveWeekdays();

                                var timeRange = xSchedule
                                    .RetrieveRangeTime();

                                var scheduleTags = xSchedule
                                    .RetrieveTags(commons);

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
                                        var xTermBreakRange = xTermBreak.RetrieveDateRange(commons);

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
