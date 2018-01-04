using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators.Instances
{
    public class GenerateFromFileClasses : GenerateFromFile, IGenerateFromFile
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
                        ?? throw new Exception($"Missing schedule");

                        var rangeTime = xSerial
                            .RetrieveRangeTimes(caches)
                            .SingleOrDefault()
                        ?? throw new Exception($"Missing rangeTime");

                        var generator = GenerateFromFileFactory.GetXSchedule(xSchedule.Name.LocalName);

                        var schedule = (ISchedule) generator.Generate(xSchedule, caches, clock);

                        var serial = new Serial.Builder
                        {
                            Schedule = schedule,
                            RangeTime = rangeTime,
                            TimeZoneProvider = timeZoneProvider,
                        }.Build();

                        serial
                            .Tags
                            .AddRange(classTags);

                        yield return serial;
                    }
                }

                yield return organisation;
            }
        }
    }
}
