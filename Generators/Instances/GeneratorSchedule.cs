using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Scheduler;
using Scheduler.Persistance;

namespace Generators.Instances
{
    public class GeneratorSchedule : IGenerator
    {
        public IEnumerable<IVertex> Generate(string sourceFile)
        {
            var xSource = XDocument
                .Load(sourceFile);

            var generatorSource = new GeneratorSource
            {
                Xml = xSource.ToString(),
                GeneratorType = "holidays"
            };

            yield return generatorSource;

            Utilities.ExpandReferences(xSource);

            var xGenerators = xSource
                .Elements("generators")
                .Elements("generator");

            foreach (var xGenerator in xGenerators)
            {
                var xGeneratorTerms = xGenerator
                    .Elements("terms")
                    .Elements("term")
                    .ToList();

                foreach (var xTerm in xGeneratorTerms)
                {
                    var date = Utilities.RetrieveDates(xTerm)
                        .ToList();
                }
            }
        }
    }
}