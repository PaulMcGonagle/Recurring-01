using Scheduler.Persistance;
using Scheduler.Ranges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Generators
{
    public interface IGeneratorX
    {
        IVertex Generate(XElement xInput);
    }
}
