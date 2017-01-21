using Scheduler.Persistance;

namespace Scheduler
{
    public interface ITag : IVertex
    {
        string Value { get; set; }
    }
}