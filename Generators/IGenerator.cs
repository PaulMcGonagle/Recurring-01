using System.Collections.Generic;
using Scheduler.Persistance;

namespace Generators
{
    public interface IGenerator
    {
        IEnumerable<IVertex> Generate(string sourceFile);
    }
}
