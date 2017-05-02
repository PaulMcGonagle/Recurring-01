using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
