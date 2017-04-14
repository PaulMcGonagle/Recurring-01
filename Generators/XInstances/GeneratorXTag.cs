using Scheduler.Persistance;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Generators.XInstances
{
    public class GeneratorXTag : IGeneratorX
    {
        public bool TryGenerate(XElement xRangeDate, IDictionary<string, IVertex> caches, out IVertex vertex, string elementsName = null)
        {
            vertex = xRangeDate.RetrieveTag();

            vertex.Connect(xRangeDate.RetrieveTags(caches, elementsName));

            return true;
        }
    }
}
