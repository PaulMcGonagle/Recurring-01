using System.Collections.Generic;
using System.Xml.Linq;
using Generators.XScheduleInstances;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators.Instances
{
    public class GenerateFromFileTerms : GenerateFromFile, IGenerateFromFile
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

            var xTerms = xGenerator
                .Element("terms");

            var generator = new GeneratorXCompositeSchedule();

            var compositeSchedule = (ISchedule)generator.Generate(xTerms, caches, elementsName: "term");

            yield return compositeSchedule;
        }
    }
}
