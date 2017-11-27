using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using CoreLibrary;
using Scheduler.Persistance;

namespace Generators
{
    public static class Utilities
    {
        public static void ExpandSource(this XElement source, out IDictionary<string, IVertex> outputCaches)
        {
            Guard.AgainstNull(source, nameof(source));

            var descendants = source
                .DescendantsAndSelf()
                .ToArray();

            var caches = descendants
                .Elements("cache")
                .ToArray();

            var references = descendants
                .Elements("reference")
                .ToArray();

            foreach (var reference in references)
            {
                var parent = reference.Parent;

                if (parent == null)
                    break;

                var referreds = source.RetrieveXReferences(reference, null);

                foreach (var referred in referreds)
                {
                    parent.Add(referred);
                }

                reference.Remove();
            }

            outputCaches = new Dictionary<string, IVertex>();

            if (!caches.Any())
                return;

            var remaining = new Queue<XElement>(caches);

            var itersSinceSucess = 0;

            while (remaining.Count > 0 && remaining.Count > itersSinceSucess)
            {
                var cache = remaining.Dequeue();

                if (cache.TryExpandLinks(new XDocument(source), outputCaches))
                {
                    remaining.Enqueue(cache);
                    itersSinceSucess++;
                }
                else
                {
                    itersSinceSucess = 0;
                }
            }
        }

        public static bool TryExpandLinks(
            this XElement xElement,
            XDocument xInput,
            IDictionary<string, IVertex> caches)
        {
            var name = xElement.RetrieveAttributeValue("name");
            var type = xElement.RetrieveAttributeValue("type");
            var path = xElement.RetrieveAttributeValue("path");

            var generatorX = GeneratorFactory.GetX(type);

            var xLink = xInput
                .XPathSelectElement(path);

            if (xLink == null)
                throw new Exception($"Unable to expand cache name {name} with path {path}");

            var link = generatorX.Generate(xLink, caches);

            if (link is null)
            {
                return false;
            }

            caches.Add(name, link);

            return true;
        }
    }
}
