using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public interface IEdgeDate
    {
        IDate Date { get; set; }

        void Save(IArangoDatabase db, IClock clock, IVertex fromVertex, string label = null);
    }
}