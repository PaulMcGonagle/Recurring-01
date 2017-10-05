using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public interface IRelation
    {
        string Label { get; set; }

        IVertex ToVertex { get; set; }

        IVertex FromVertex { get; set; }

        void Save(IArangoDatabase db, IClock clock);
    }
}