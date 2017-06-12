using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators
{
    public abstract class GenerateFromFile
    {
        protected void GenerateSetup(
            string generatorType, 
            string sourceFile, 
            IClock clock, 
            out XElement xGenerator, 
            out IGeneratorSource generatorSource, 
            out IDictionary<string, IVertex> caches)
        {
            var xSource = XDocument
                .Load(sourceFile);

            xSource.ExpandReferences();
            caches = xSource.Root.ExpandLinks();

            generatorSource = new GeneratorSource
            {
                Xml = xSource.ToString(),
                GeneratorType = generatorType
            };

            xGenerator = xSource
                .Element("generator");

            if (xGenerator == null)
                throw new Exception($"SourceFile does not contain a generator '{sourceFile}'");
        }
    }
}
