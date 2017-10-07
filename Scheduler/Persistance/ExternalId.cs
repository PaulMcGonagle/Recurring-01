using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class ExternalId : Vertex, IExternalId
    {
        public ExternalId()
        {
            Uid = Guid.NewGuid().ToString();
        }

        public ExternalId(string uid)
        {
            Uid = uid;
        }

        public string Uid { get; private set; }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ExternalId>(db);
            base.Save(db, clock);
        }

        #endregion
    }
}
