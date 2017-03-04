using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public class Instances : List<IInstance>
    {
        public Instances() { }

        public Instances(IEnumerable<IInstance> instances)
        {
            AddRange(instances);
        }

        public void Save(IArangoDatabase db, IClock clock)
        {
            this.ForEach(e => e.Save(db, clock));
        }
    }
}
