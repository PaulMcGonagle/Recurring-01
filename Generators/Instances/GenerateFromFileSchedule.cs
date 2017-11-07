using System;
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
        public IEnumerable<IVertex> Generate(string sourceFile, IClock clock)
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

            var generator = GeneratorFactory.GetX("CompositeSchedule");

            var vertex = generator.Generate(xSchedules, caches, clock: clock);

            var compositeSchedule = vertex as ICompositeSchedule;

            if (compositeSchedule == null)
                throw new Exception($"Generator generated invalid type. Expected ICompositeSchedule, returned {vertex.GetType()}");

            compositeSchedule
                .Connect(xSchedules.RetrieveTags(caches));

            generatorSource
                .Schedules
                .Add(new EdgeSchedule(compositeSchedule, "Generated"));

            yield return compositeSchedule;
        }
    }
}