using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators
{
    public abstract class GenerateFromFile : IGenerateFromFile
    {
        protected void GenerateSetup(
            string generatorType, 
            string sourceFile, 
            IClock clock, 
            out XElement xGenerator, 
            out IGeneratorSource generatorSource, 
            out IDictionary<string, IVertex> caches)
        {
            Guard.AgainstNullOrWhiteSpace(sourceFile, nameof(sourceFile));

            var xSource = XDocument
                .Load(sourceFile);

            Guard.AgainstNull(xSource, nameof(xSource));
            Guard.AgainstNull(xSource.Root, nameof(xSource.Root));

            if (xSource
                .Root
                ?.Name != "generator")
                throw new ArgumentException("Unexpected root node", nameof(xSource.Root));

            xSource
                .Root
                .ExpandSource(out caches);

            generatorSource = new GeneratorSource
            {
                Xml = xSource.ToString(),
                GeneratorType = generatorType,
                Caches = new EdgeVertexs<IVertex>(
                    caches
                        .Select(cache => new EdgeVertex<IVertex>(cache.Value, "HasCache")))
            };

            xGenerator = xSource
                .Element("generator");

            if (xGenerator == null)
                throw new Exception($"SourceFile does not contain a generator '{sourceFile}'");
        }

        public abstract IEnumerable<IVertex> Generate(string sourceFile, IClock clock);
    }
}
