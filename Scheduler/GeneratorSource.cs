using System.Runtime.Serialization;
using NodaTime;
using Scheduler.Persistance;
using ArangoDB.Client;

namespace Scheduler
{
    public class GeneratorSource : Vertex, IGeneratorSource
    {
        public string Xml { get; set; }
        public string GeneratorType { get; set; }

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> Schedules { get; set; }

        [IgnoreDataMember]
        public IEdgeVertexs<IVertex> Caches { get; set; }

        public GeneratorSource()
        {
            Schedules = new EdgeVertexs<ISchedule>();
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<GeneratorSource>(db);
            Schedules.Save(db, clock, this);
            Caches?.Save(db, clock, this, "HasCache");
            base.Save(db, clock);
        }

        #endregion
    }
}
