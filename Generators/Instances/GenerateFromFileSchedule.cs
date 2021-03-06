﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Generators.Instances
{
    public class GenerateFromFileSchedule : GenerateFromFile, IGenerateFromFile
    {
        public override IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            GenerateSetup(
                generatorType: "schedule",
                sourceFile: sourceFile,
                clock: clock,
                xGenerator: out XElement xGenerator,
                generatorSource: out IGeneratorSource generatorSource,
                caches: out IDictionary <string, IVertex> caches);

            yield return generatorSource;

            var xSchedules = xGenerator
                .Element("schedules");

            var generator = GenerateFromFileFactory.GetX("CompositeSchedule");

            var vertex = generator.Generate(xSchedules, caches, clock: clock);

            if (!(vertex is ISchedule schedule))
                throw new Exception($"Generator generated invalid type. Expected ISchedule, returned {vertex.GetType()}");

            schedule
                .Connect(xSchedules.RetrieveTags(caches));

            generatorSource
                .Schedules
                .Add(new EdgeSchedule(schedule, "Generated"));

            yield return schedule;
        }
    }
}