using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
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
                .XPathSelectElements(".//cache");

            var links = new Dictionary<string, IVertex>();

            foreach (var xElement in xElements)
            {
                var name = xElement.RetrieveAttributeValue("name");
                var type = xElement.RetrieveAttributeValue("type");
                var path = xElement.RetrieveAttributeValue("path");

                var generatorX = GeneratorFactory.GetX(type);

                var xLink = xInput
                    .XPathSelectElement(path);

                var link = generatorX.Generate(xLink, null);

                links.Add(name, link);
            }

            return links;
        }
    }
}
