using System.Collections.Generic;
using NodaTime;
using Scheduler.Persistance;

namespace Generators
{
    public interface IGenerateFromVertex
    {
        IEnumerable<IVertex> Generate(IVertex vertex, IClock clock);
    }
}
