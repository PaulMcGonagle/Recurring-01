using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;

namespace Scheduler
{
    public interface IPersistableEntry
    {
        void Save(IArangoDatabase db);
    }
}
