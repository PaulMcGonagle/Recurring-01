using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Generators.XInstances;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Generators.Instances
{
    public class GenerateFromFileClasses : GenerateFromFile, IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            GenerateSetup(
                generatorType: "classes",
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

                    var xSchedules = xClass
                        .Elements("schedules")
                        .SingleOrDefault();

                    var xRangeTimes = xClass
                        .Element("rangeTimes")
                        .RetrieveRangeTimes(caches)
                        .ToList();

                    var serials = new Serials();

                    var generator = new GeneratorXCompositeSchedule();


                    var compositeSchedule = (ISchedule)generator.Generate(xSchedules, caches);

                    compositeSchedule
                        .Tags
                        .AddRange(classTags);

                    serials
                        .AddRange(xRangeTimes.Select(rt => new Serial(compositeSchedule, new EdgeRangeTime(rt), timeZoneProvider)));

                    foreach (var serial in serials)
                    {
                        yield return serial;
                    }

                    //foreach (var xSchedule in xSchedules)
                    //{
                    //        var weekdays = xSchedule
                    //            .RetrieveWeekdays();

                    //        var rangeTime = xSchedule
                    //            .RetrieveRangeTime();

                    //        var byWeekdays = ByWeekdays
                    //            .Create(
                    //                weekdays: weekdays,
                    //                rangeDate: termRange);

                    //        ISchedule schedule;

                    //        if (xTermBreaks.Count > 0)
                    //        {
                    //            var compositeSchedule = CompositeSchedule
                    //                .Create(
                    //                    schedule: byWeekdays,
                    //                    rangeDate: termRange);

                    //            foreach (var xTermBreak in xTermBreaks)
                    //            {
                    //                var xTermBreakRanges = xTermBreak
                    //                    .RetrieveRangeDates(caches)
                    //                    .ToList();

                    //                compositeSchedule.Breaks.AddRange(xTermBreakRanges.Select(br => new EdgeVertex<IRangeDate>(br)));
                    //            }

                    //            schedule = compositeSchedule;
                    //        }
                    //        else
                    //        {
                    //            schedule = byWeekdays;
                    //        }

                    //        schedule.Connect(termTags);

                    //        var serial = new Serial(
                    //            schedule: schedule,
                    //            rangeTime: new EdgeRangeTime(rangeTime),
                    //            timeZoneProvider: timeZoneProvider);

                    //        var serialTags = termTags;

                    //        serial.Tags  = new EdgeVertexs<ITag>(serialTags) {new EdgeVertex<ITag>(termTag)};

                    //        serials.Add(serial);
                    //    }

                    //    var @event = new Event
                    //    {
                    //        Title = organisation.Value + "." + termName + "." + groupName + "." + className,
                    //        Serials = new EdgeVertexs<ISerial>(serials),
                    //        Tags = new EdgeVertexs<ITag>(classTags),
                    //}

                    yield return compositeSchedule;
                    //}
                    //yield return @event;
                }

                yield return organisation;
            }
        }
    }
}
