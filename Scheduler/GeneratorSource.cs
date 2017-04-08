using NodaTime;
using Scheduler.Persistance;
using ArangoDB.Client;

namespace Scheduler
{
    public class GeneratorSource : Vertex, IGeneratorSource
    {
        public string Xml { get; set; }
        public string GeneratorType { get; set; }

        public IEdgeVertexs<ISchedule> Schedules { get; set; }

        public GeneratorSource()
        {
            Schedules = new EdgeVertexs<ISchedule>();
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<GeneratorSource>(db);
            Schedules.Save(db, clock, this);
            base.Save(db, clock);
        }

        #endregion
    }
}
