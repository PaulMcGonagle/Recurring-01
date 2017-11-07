using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Generators.XInstances;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Generators.Instances
{
    public class GenerateFromFileRangeDates : GenerateFromFile, IGenerateFromFile
    {
        public IEnumerable<IVertex> Generate(
            string sourceFile,
            IClock clock)
        {
            GenerateSetup(
                generatorType: "ranges",
                sourceFile: sourceFile,
                clock: clock,
                xGenerator: out XElement xGenerator,
                generatorSource: out IGeneratorSource generatorSource,
                caches: out IDictionary<string, IVertex> caches);

            yield return generatorSource;

            var tagBaseType = new Tag(ident: "baseType", value: "Range");

            yield return tagBaseType;

            var xRanges = xGenerator
                .Elements("ranges")
                .Elements("range")
                .ToList();

            foreach (var xRange in xRanges)
            {
                var generatorRangeDate = new GeneratorXRangeDate();

                var rangeDate = (IRangeDate)generatorRangeDate
                    .Generate(xRange, caches, null, clock);

                var rangeTags = xRange
                    .RetrieveTags(caches)
                    .ToList();

                tagBaseType
                    .Connect(rangeTags.SingleOrDefault(ct => ct.Ident == "name"));

                rangeDate.Connect(tagBaseType);

                yield return rangeDate;
            }
        }
    }
}
