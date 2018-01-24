using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.Instances
{
    public class GenerateFromFileClasses : GenerateFromFile, IGenerateFromFile
    {
        public override IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
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

                    var xSerials = xClass
                        .Element("serials")
                        ?? throw new Exception($"Missing serials");

                    foreach (var xSerial in xSerials
                        .Elements("serial"))
                    {
                        var xSchedule = xSerial
                            .Elements("schedule")
                            .Elements()
                            .SingleOrDefault()
                            ?? throw new Exception($"Missing serial schedule instance");

                        var rangeTime = xSerial
                            .RetrieveRangeTimes(caches)
                            .SingleOrDefault()
                            ?? throw new Exception($"Missing serial rangeTime");

                        var generator = GenerateFromFileFactory.GetXSchedule(xSchedule);

                        var vertex = generator.Generate(xSchedule, caches, clock);

                        if (!(vertex is ISchedule schedule))
                            throw new Exception($"Generator generated invalid type. Expected ISchedule, returned {vertex.GetType()}");

                        foreach (var xTerm in xSerial
                            .Elements("terms")
                            .Elements("term"))
                        {
                            var xTermSchedule = xTerm
                                .Element("schedule")
                                ?.Elements()
                                .SingleOrDefault()
                                ?? throw new Exception($"Missing term schedule");

                            var termSchedule = GenerateFromFileFactory
                                .GetXSchedule(xTermSchedule)
                                .Generate(xTermSchedule, caches, clock) as ISchedule
                                ?? throw new Exception("Could not generate termSchedule");

                            var termBreakVertexs = xTerm
                                .Elements("breaks")
                                .Elements("schedule")
                                .Select(xBreakSchedule => xBreakSchedule.Elements().SingleOrDefault())
                                .Select(xBreakScheduleInstance => GenerateFromFileFactory
                                    .GetXSchedule(xBreakScheduleInstance)
                                    .Generate(xBreakScheduleInstance, caches, clock))
                                .ToList();

                            if (!termBreakVertexs.TrueForAll(v => v is ISchedule))
                                throw new Exception("Generated vertex not an ISchedule");

                            var termBreaks = termBreakVertexs.ConvertAll(v => (ISchedule)v);

                            var compositeSchedule = new CompositeSchedule.Builder
                            {
                                Inclusion = termSchedule,
                                Exclusions = new EdgeVertexs<ISchedule>(termBreaks),
                            }.Build();

                            var filteredSchedule = new FilteredSchedule
                            {
                                Inclusion = new EdgeVertex<ISchedule>(new Schedule(compositeSchedule)),
                                Filters = new EdgeVertexs<ISchedule>(schedule),
                            };

                            var serial = new Serial.Builder
                            {
                                Schedule = new Schedule(filteredSchedule),
                                RangeTime = rangeTime,
                                TimeZoneProvider = timeZoneProvider,
                            }.Build();

                            serial
                                .Tags
                                .AddRange(classTags);

                            yield return serial;
                        }
                    }
                }

                yield return organisation;
            }
        }
    }
}
