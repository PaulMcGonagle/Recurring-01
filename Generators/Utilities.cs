using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Scheduler.Persistance;

namespace Generators
{
    public static class Utilities
    {
        public static void ExpandReferences(this XDocument xInput)
        {
            var xElements = xInput
                .Root
                ?.DescendantsAndSelf()
                .ToList();

            if (xElements == null)
            {
                return;
            }

            foreach (var xElement in xElements)
            {
                var xElementReferences = xElement
                    .Elements("reference")
                    .ToList();

                foreach (var xElementReference in xElementReferences)
                {
                    var type = xElementReference
                        .Attribute("type")?.Value;

                    var xReferencedTags = xInput
                        .RetrieveXReferences(xElementReferences, type)
                        .ToList();

                    xElement.Add(xReferencedTags);
                }

                xElementReferences.Remove();
            }
        }

        public static IDictionary<string, IVertex> ExpandLinks(this XDocument xInput)
        {
            var xElements = xInput
                .Root
                ?.DescendantsAndSelf()
                .ToList();

            var links = new Dictionary<string, IVertex>();

            if (xElements == null)
            {
                return links;
            }

            foreach (var xElement in xElements)
            {
                var xElementReferences = xElement
                    .Elements("cache")
                    .ToList();

                foreach (var xElementReference in xElementReferences)
                {
                    var type = xElementReference.RetrieveValue("type");
                    var name = xElementReference.RetrieveValue("name");

                    var generatorX = GeneratorFactory.GetX(type);

                    var xReferencedTags = xInput
                        .RetrieveXReference(xElementReference, type);

                    var link = generatorX.Generate(xReferencedTags, null);

                    links.Add(name, link);
                }
            }

            return links;
        }
    }
}
