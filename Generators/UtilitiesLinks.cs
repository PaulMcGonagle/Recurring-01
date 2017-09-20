using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Scheduler.Persistance;

namespace Generators
{
    public static class UtilitiesLinks<T> where T : class, IVertex
    {
        public static IEnumerable<T> Retrieve(
            XElement xInput,
            IDictionary<string, IVertex> caches,
            string elementsName = "link")
        {
            var xLinks = xInput
                .Elements(elementsName)
                .ToList();

            foreach (var xLink in xLinks)
            {
                var cacheName = xLink.RetrieveValue("cache");

                if (!caches.TryGetValue(cacheName, out IVertex commonVertex))
                    throw new ArgumentException($"No cache found with name {cacheName}");

                var output = commonVertex as T;

                if (output == null)
                    throw new Exception($"Common link does not return a valid {typeof(T)}");

                yield return output;
            }
        }

        public static IEnumerable<T> Retrieve(IEnumerable<XElement> xInput, IDictionary<string, IVertex> caches)
        {
            var xLinks = xInput
                .Where(i => i.Name == "link")
                .ToList();

            foreach (var xLink in xLinks)
            {
                var cacheName = xLink.RetrieveValue("cache");

                if (!caches.TryGetValue(cacheName, out IVertex commonVertex))
                    throw new ArgumentException($"No cache found with name {cacheName}");

                var output = commonVertex as T;

                if (output == null)
                    throw new Exception($"Common link does not return a valid {typeof(T)}");

                yield return output;
            }
        }
    }
}
