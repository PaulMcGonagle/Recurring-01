using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators
{
    public abstract class GenerateFromFile
    {
        protected void GenerateSetup(
            string sourceFile, 
            IClock clock, 
            string generatorType, 
            out XElement xGenerator, 
            out IGeneratorSource generatorSource, 
            out XDocument xSource, 
            out IDictionary<string, IVertex> caches)
        {
            xSource = XDocument
                .Load(sourceFile);

            xSource.ExpandReferences();
            caches = xSource.ExpandLinks();

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
