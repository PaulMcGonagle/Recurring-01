using Scheduler.Persistance;
using System.Collections.Generic;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;

namespace Generators.XInstances
{
    public class GeneratorXTag : IGeneratorX
    {
        public IVertex Generate(XElement xInput, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xInput, nameof(xInput));
            Guard.AgainstNull(caches, nameof(caches));

            var xTag = xInput.RetrieveTag();

            xTag.Connect(xInput.RetrieveTags(caches, elementsName ?? "tags"));

            return xTag;
        }
    }
}
