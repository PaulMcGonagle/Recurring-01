using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Serials : List<Serial>, ISerial
    {
        public IEnumerable<Episode> Episodes
        {
            get { return this.SelectMany(ce => ce.Episodes); }
        }

        public Vertex.SaveResult Save(IArangoDatabase db)
        {
            foreach (var serial in this)
            {
                var result = serial.Save(db);

                if (result != Vertex.SaveResult.Success)
                    return result;
            }

            return Vertex.SaveResult.Success;
        }
    }
}
