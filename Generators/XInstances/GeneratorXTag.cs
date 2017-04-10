using Scheduler.Persistance;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Generators.XInstances
{
    public class GeneratorXTag : IGeneratorX
    {
        public IVertex Generate(XElement xRangeDate, IDictionary<string, IVertex> caches, string elementsName = null)
        {
            var tag = xRangeDate.RetrieveTag();

            tag.Connect(xRangeDate.RetrieveTags(caches, elementsName));

            return tag;
        }
    }
}
