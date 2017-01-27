using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISerial : IVertex
    {
        IEpisodes Episodes { get; }

        IEdgeVertexs<ITag> Tags { get; set; }
    }
}
