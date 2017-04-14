using System;
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
                        .Attribute("type")
                        ?.Value;

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
            var caches = xInput
                .XPathSelectElements(".//cache")
                .ToList();

            var links = new Dictionary<string, IVertex>();

            var remaining = new Queue<XElement>(caches);

            var itersSinceSucess = 0;

            while (remaining.Count > 0 && remaining.Count > itersSinceSucess)
            {
                var cache = remaining.Dequeue();

                if (!cache.TryExpandLinks(xInput, links))
                {
                    remaining.Enqueue(cache);
                    itersSinceSucess++;
                }
                else
                {
                    itersSinceSucess = 0;
                }
            }

            return links;
        }

        public static bool TryExpandLinks(
            this XElement xElement, 
            XDocument xInput,
            IDictionary<string, IVertex> cache)
        {
            var name = xElement.RetrieveAttributeValue("name");
            var type = xElement.RetrieveAttributeValue("type");
            var path = xElement.RetrieveAttributeValue("path");

            var generatorX = GeneratorFactory.GetX(type);

            var xLink = xInput
                .XPathSelectElement(path);

            if (xLink == null)
                throw new Exception($"Unable to expand cache name {name} with path {path}");

            if (!generatorX.TryGenerate(xLink, cache, out IVertex link))
            {
                return false;
            }

            cache.Add(name, link);

            return true;
        }
    }
}
