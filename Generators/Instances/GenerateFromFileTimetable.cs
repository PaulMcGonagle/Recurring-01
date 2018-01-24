using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Generators.XInstances;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GenerateFromFileTimetable : GenerateFromFile
    {
        private class GroupClass : IComparable<GroupClass>
        {
            public IsoDayOfWeek IsoDayOfWeek { get; set; }
            public LocalTime Start { get; set; }
            public LocalTime End { get; set; }

            public string Name { get; set; }
            public IList<ITag> Tags { get; set; }

            public static GroupClass Generate(XElement xClass, IDictionary<string, IVertex> caches)
            {
                var groupClass = new GroupClass
                {
                    Start = xClass.RetrieveAttributeAsLocalTime("start"),
                    Name = xClass.RetrieveAttributeValue("name"),
                    IsoDayOfWeek = xClass
                        .Parent
                        .TryRetrieveAttributeAsWeekday("weekday", out IsoDayOfWeek isoDayOfWeek)
                        ? isoDayOfWeek
                        : IsoDayOfWeek.None,
                    Tags = xClass
                        .RetrieveTags(caches)
                        .ToList(),
                };

                return groupClass;
            }

            public int CompareTo(GroupClass gc)
            {
                return Start.CompareTo(gc.Start);
            }
        }

        public override IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            GenerateSetup(
                generatorType: "timetables",
                sourceFile: sourceFile,
                clock: clock,
                xGenerator: out XElement xGenerator,
                generatorSource: out IGeneratorSource generatorSource,
                caches: out IDictionary<string, IVertex> caches);

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

            var days = xGenerator
                .Elements("weekdays")
                .Elements("weekday")
                .Select(day =>
                {
                    var isoDayOfWeek = day
                        .RetrieveAttributeAsIsoDayOfWeek("name");

                    IRangeTime rangeTime = day
                        .Elements("rangeTime")
                        .SingleOrDefault()
                        ?.RetrieveRangeTime(caches);

                    return new Tuple<IsoDayOfWeek, IRangeTime>(isoDayOfWeek, rangeTime);
                })
                .ToList();

            foreach (var xGroup in xGroups)
            {
                var groupName = xGroup
                    .RetrieveAttributeValue("name");

                var group = organisation
                    .Connect("group", groupName);

                var groupTags = xGroup
                    .RetrieveTags(caches)
                    .ToList();

                group.Connect(groupTags);

                var rangeTime = xGroup
                    .Elements("rangeTime")
                    .SingleOrDefault()
                    ?.RetrieveRangeTime(caches);

                var terms = xGroup
                    .Element("terms")
                    ?.Elements("term")
                    .Select(xTerm => new GeneratorXTerm().Generate(xTerm, caches, clock) as ISchedule)
                    .ToList();

                var classes = days
                    .SelectMany(d => xGroup
                        .Elements("classes")
                        .Elements("class")
                        .Select(xClass => new { IsoDayOfWeek = d.Item1, GroupClass = GroupClass.Generate(xClass, caches)})
                        .Where(f => f.GroupClass.IsoDayOfWeek == f.IsoDayOfWeek || f.GroupClass.IsoDayOfWeek == IsoDayOfWeek.None)
                        .Select(f => new GroupClass
                            {
                                Start = f.GroupClass.Start,
                                Name = f.GroupClass.Name,
                                IsoDayOfWeek = f.GroupClass.IsoDayOfWeek == IsoDayOfWeek.None ? f.IsoDayOfWeek : f.GroupClass.IsoDayOfWeek,
                                Tags = f.GroupClass.Tags,
                            }
                        ))
                    .ToList();

                var groupedDayClasses = classes
                    .GroupBy(dc => dc.IsoDayOfWeek)
                    .ToList();

                foreach (var groupedDayClass in groupedDayClasses)
                {
                    var classForDay = groupedDayClass
                        .ToArray();

                    var dayEnd = days
                        .SingleOrDefault(day => day.Item1 == groupedDayClass.Key)
                        ?.Item2
                        ?.End;

                    if (!dayEnd.HasValue)
                    {
                        dayEnd = rangeTime
                            ?.End
                            ?? throw new Exception($"Missing End time for day {groupedDayClass.Key}");
                    }
                    Array.Sort(classForDay);

                    var length = classForDay.Length;

                    for (var i = 0; i < length; i++)
                    {
                        classForDay[i].End = i < length - 1 ? classForDay[i + 1].Start : dayEnd.Value;
                    }

                    var serials = classForDay
                        .SelectMany(c3 => terms
                            ?.Select(ter => new Serial.Builder
                                {
                                    Schedule = new Schedule(new FilteredSchedule
                                    {
                                        Inclusion = new EdgeSchedule(ter.ScheduleInstance),
                                        Filter = new Schedule(new ByWeekdays
                                            {
                                                Weekdays = new [] 
                                                { c3.IsoDayOfWeek }
                                            })
                                    }),
                                    RangeTime = new RangeTime.Builder
                                    {
                                        Start = c3.Start,
                                        End = c3.End,
                                    }.Build(),
                                    TimeZoneProvider = timeZoneProvider,
                                    Tags = c3.Tags.Union(new[] { new Tag("name", c3.Name), }),
                            }.Build()
                            )
                        ).ToList();

                    foreach (var serial in serials)
                    {
                        serial.Connect(group);

                        yield return serial;
                    }
                }
            }
        }
    }
}
