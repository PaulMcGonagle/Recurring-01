using Scheduler.Persistance;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;

namespace Generators.XInstances
{
    public class GeneratorXTag : IGeneratorX
    {
        public IVertex Generate(XElement xRangeDate, IDictionary<string, IVertex> caches, string elementsName = null, IClock clock = null)
        {
            var xTag = xRangeDate.RetrieveTag();

            xTag.Connect(xRangeDate.RetrieveTags(caches, elementsName));

            return xTag;
        }
    }
}
