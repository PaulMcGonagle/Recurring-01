using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Scheduler.Persistance;

namespace Generators
{
    public static class UtilitiesLinks<T> where T : class, IVertex
    {
        public static IEnumerable<T> RetrieveAll(
            XElement xInput,
            IDictionary<string, IVertex> caches,
            string elementName = "link")
        {
            var xLinks = xInput
                .Elements(elementName)
                .ToList();

            foreach (var xLink in xLinks)
            {
                var output = Retrieve(xLink, caches);

                if (output == null)
                    throw new Exception($"Common link does not return a valid {typeof(T)}");

                yield return output;
            }
        }

        public static T Retrieve(XElement xInput, IDictionary<string, IVertex> caches, string elementName = "link")
        {
            if (xInput.Name.LocalName != elementName)
                throw new Exception($"Unexpected name: expected {elementName}, found {xInput.Name}");

            var cacheName = xInput.RetrieveValue("cache");

            return Retrieve(cacheName, caches);
        }

        public static T Retrieve(
            string cacheName,
            IDictionary<string, IVertex> caches)
        {
            if (!caches.TryGetValue(cacheName, out IVertex commonVertex))
                throw new ArgumentException($"No cache found with name {cacheName}");

            var output = commonVertex as T;

            if (output == null)
                throw new Exception($"Common link does not return a valid {typeof(T)}");

            return output;
        }
    }
}
