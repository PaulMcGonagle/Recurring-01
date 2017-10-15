using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public interface IEdge : IVertex
    {
        string FromId { get; set; }
        IVertex FromVertex { get; set; }
        string Label { get; set; }
        string ToId { get; set; }
        IVertex ToVertex { get; set; }

        void Rehydrate(IArangoDatabase db);
        void Save(IArangoDatabase db, IClock clock);
        void Save(IArangoDatabase db, IClock clock, IVertex fromVertex);
        string ToString();
    }
}