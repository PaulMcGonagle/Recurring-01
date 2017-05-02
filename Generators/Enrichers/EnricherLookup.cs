using System.Collections.Generic;
using NodaTime;
using Scheduler.Persistance;

namespace Generators.Enrichers
{
    public class EnricherLookup : IEnricher
    {
        public void Go(
            IEnumerable<IVertex> vertexs,
            string ident,
            Dictionary<LocalDate, IVertex> lookup)
        {
            
        }

    }
}
