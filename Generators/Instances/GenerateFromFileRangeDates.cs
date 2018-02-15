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
    public class GenerateFromFileRangeDates : GenerateFromFile
    {
        public override IEnumerable<IVertex> Generate(
            string sourceFile,
            IClock clock)
        {
            GenerateSetup(
                generatorType: "rangeDates",
                sourceFile: sourceFile,
                clock: clock,
                xGenerator: out XElement xGenerator,
                generatorSource: out IGeneratorSource generatorSource,
                caches: out IDictionary<string, IVertex> caches);

            yield return generatorSource;

            var tagBaseType = new Tag(ident: "baseType", value: "RangeDate");

            yield return tagBaseType;

            var xRanges = xGenerator
                .Elements("rangeDates")
                .Elements("rangeDate")
                .ToList();

            foreach (var xRange in xRanges)
            {
                var generatorRangeDate = new GeneratorXRangeDate();

                var rangeDate = (IRangeDate)generatorRangeDate
                    .Generate(xRange, caches, clock: clock);

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
