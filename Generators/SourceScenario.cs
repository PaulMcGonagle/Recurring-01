using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using Scheduler.Persistance;

namespace Generators
{
    public abstract class SourceScenario
    {
        protected IClock Clock;

        public IEnumerable<IVertex> Vertexs { get; protected set; } = new List<IVertex>();
    }
}
