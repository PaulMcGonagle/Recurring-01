using System.Collections.Generic;
using System.Linq;
using Scheduler.Persistance;

namespace Generators
{
    public static class GenerateFromFile
    {
        public static IEnumerable<IVertex> Go(
            string sourceFile, 
            string generatorType)
        {
            var generator = GeneratorFactory.Get(generatorType);

            var vertexs = generator.Generate(
                    sourceFile: "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\Generators\\Sources\\HG.xml",
                    clock: null)
                .ToList();

            return vertexs;
        }
    }
}
