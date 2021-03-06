﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Generators.XScheduleInstances;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators.Instances
{
    public class GenerateFromFileOffset : GenerateFromFile, IGenerateFromFile
    {
        public override IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
        {
            GenerateSetup(
                generatorType: "offset",
                sourceFile: sourceFile,
                clock: clock,
                xGenerator: out XElement xGenerator,
                generatorSource: out IGeneratorSource generatorSource,
                caches: out IDictionary<string, IVertex> caches);

            yield return generatorSource;

            var xSchedule = xGenerator
                .Elements("schedules")
                .SingleOrDefault();

            var generator = new GeneratorXCompositeSchedule();

            var schedule = (ISchedule)generator.Generate(xSchedule, caches);

            yield return schedule;
        }

    }
}
