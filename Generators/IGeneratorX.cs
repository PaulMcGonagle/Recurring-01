using Scheduler.Persistance;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Generators
{
    public interface IGeneratorX
    {
        bool TryGenerate(XElement xInput, IDictionary<string, IVertex> caches, out IVertex vertex, string elementsName = null);
    }
}
