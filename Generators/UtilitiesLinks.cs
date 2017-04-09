using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Scheduler.Persistance;

namespace Generators
{
    public static class UtilitiesLinks<T> where T : class, IVertex
    {
        public static IEnumerable<T> Retrieve(XElement xInput, IDictionary<string, IVertex> commons, string elementsName)
        {
            var xLinks = xInput
                .Elements(elementsName)
                .Elements("link")
                .ToList();

            foreach (var xLink in xLinks)
            {
                var commonName = xLink.RetrieveValue("common");

                IVertex commonVertex;

                if (!commons.TryGetValue(commonName, out commonVertex))
                    throw new ArgumentException($"No common found with name {commonName}");

                var dateRange = commonVertex as T;

                if (dateRange == null)
                    throw new Exception("Common link does not return a date range");

                yield return dateRange;
            }
        }
    }
}
