using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using Scheduler.Persistance;

namespace Generators
{
    public interface ISourceScenario
    {
        IEnumerable<IVertex> Generate(IClock clock);
    }
}
