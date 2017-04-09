using System.Collections.Generic;
using NodaTime;
using Scheduler.Persistance;

namespace Generators
{
    public interface IGenerator
    {
        IEnumerable<IVertex> Generate(string sourceFile, IClock clock);
    }
}
