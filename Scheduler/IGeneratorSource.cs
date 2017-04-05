using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface IGeneratorSource : IVertex
    {
        string Xml { get; set; }
        string GeneratorType { get; set; }
    }
}
