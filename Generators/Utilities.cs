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
        public static void ExpandReferences(this XDocument xDocument)
        {
            if (xDocument.Root == null)
                throw new Exception("Unable to load ");

            var xReferences = xDocument
                .Root
                ?.DescendantsAndSelf()
                .Elements("reference")
                .ToArray();

            foreach (var xReference in xReferences)
            {
                var xParent = xReference.Parent;

                if (xParent == null)
                    break;

                var xReferred = xDocument.RetrieveXReference(xReference, null);

                xParent.Add(xReferred);
                
                xReference.Remove();
            }
        }

        public static IDictionary<string, IVertex> ExpandLinks(this XElement xInput)
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

                if (cache.TryExpandLinks(xInput.Document, links))
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

            var link = generatorX.Generate(xLink, cache);

            if (link is null)
            {
                return false;
            }

            cache.Add(name, link);

            return true;
        }
    }
}
