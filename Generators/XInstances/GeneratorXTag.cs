using Scheduler.Persistance;
using System.Collections.Generic;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;

namespace Generators.XInstances
{
    public class GeneratorXTag : IGeneratorX
    {
        public IVertex Generate(XElement xRangeDate, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xRangeDate, nameof(xRangeDate));
            Guard.AgainstNull(caches, nameof(caches));

            var xTag = xRangeDate.RetrieveTag();

            xTag.Connect(xRangeDate.RetrieveTags(caches, elementsName ?? "tags"));

            return xTag;
        }
    }
}
