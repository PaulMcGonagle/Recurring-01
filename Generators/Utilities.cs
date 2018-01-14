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

            ExpandCaches(source, out outputCaches);
            ExpandReferences(source);
            ExpandLinkValues(source);
        }

        private static void ExpandLinkValues(XContainer source)
        {
            var linkValues = source
                .Descendants()
                .Elements("referenceValue")
                .ToArray();

            foreach (var referenceValue in linkValues)
            {
                var parent = referenceValue.Parent;

                Guard.AgainstNull(parent, nameof(parent));

                var node = referenceValue.RetrieveAttributeValue("node");
                var path = referenceValue.RetrieveAttributeValue("path");
                var attribute = referenceValue.RetrieveAttributeValue("attribute");

                var referred = source
                    .XPathSelectElement(path);

                if (referred == null)
                    throw new Exception($"Unable to resolve referenceValue with path {path}");

                var value = referred
                    .Attribute(node);

                if (value == null)
                    throw new Exception($"Unable to resolve referenceValue.{node}");

                var target = parent
                    .Attribute(attribute);

                if (target == null)
                    throw new Exception($"Unable to resolve referenceValue.{attribute}");

                target.SetValue(value.Value);

                referenceValue.Remove();
            }
        }

        private static void ExpandReferences(XElement source)
        {
            var references = source
                .Descendants()
                .Elements("reference")
                .ToArray();

            foreach (var reference in references)
            {
                var parent = reference.Parent;

                if (parent == null)
                    break;

                var referreds = source
                    .RetrieveXReferences(reference, null)
                    .ToList();

                foreach (var referred in referreds)
                {
                    parent.Add(referred);
                }

                reference.Remove();
            }
        }

        private static void ExpandCaches(XElement source, out IDictionary<string, IVertex> outputCaches)
        {
            outputCaches = new Dictionary<string, IVertex>();

            var caches = source
                .Elements("caches")
                .Elements("cache")
                .ToArray();

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

            var generatorX = GenerateFromFileFactory.GetX(type);

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
