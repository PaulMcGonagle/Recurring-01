using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISerial : IVertex
    {
        IEpisodes Episodes { get; }
    }
}
