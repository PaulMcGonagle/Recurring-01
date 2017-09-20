using Scheduler.Persistance;

namespace Scheduler
{
    public interface ITag : IVertex
    {
        string Ident { get; set; }
        string Value { get; set; }
        string Payload { get; set; }
    }
}