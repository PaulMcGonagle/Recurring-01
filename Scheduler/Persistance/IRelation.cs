using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public interface IRelation
    {
        string Label { get; set; }

        void Save(IArangoDatabase db, IClock clock, IVertex fromVertex);
    }
}