using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;

namespace Generators
{
    public interface IGenerator
    {
        IEnumerable<IVertex> Generate(string sourceFile);
    }
}
